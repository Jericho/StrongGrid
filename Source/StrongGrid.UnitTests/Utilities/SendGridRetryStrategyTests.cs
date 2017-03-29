using Pathoschild.Http.Client;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class SendGridRetryStrategyTests
	{
		[Fact]
		public void ShouldRetry_returns_false_when_previous_response_is_null()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = (HttpResponseMessage)null;

			// Act
			var result = sendGridRetryStrategy.ShouldRetry(response);

			// Assert
			result.ShouldBeFalse();
		}

		[Fact]
		public void ShouldRetry_returns_true_when_statuscode_429()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = new HttpResponseMessage((HttpStatusCode)429);

			// Act
			var result = sendGridRetryStrategy.ShouldRetry(response);

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void ShouldRetry_returns_false_when_statuscode_not_429()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = new HttpResponseMessage(HttpStatusCode.BadGateway);

			// Act
			var result = sendGridRetryStrategy.ShouldRetry(response);

			// Assert
			result.ShouldBeFalse();
		}

		[Fact]
		public void GetNextDelay_with_null_HttpHeaders()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = (HttpResponseMessage)null;

			// Act
			var result = sendGridRetryStrategy.GetDelay(1, response);

			// Assert
			result.ShouldBe(TimeSpan.FromSeconds(1));
		}

		[Fact]
		public void CalculateDelay_without_XRateLimitReset()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = new HttpResponseMessage((HttpStatusCode)428);

			// Act
			var result = sendGridRetryStrategy.GetDelay(1, response);

			// Assert
			result.ShouldBe(TimeSpan.FromSeconds(1));
		}

		[Fact]
		public void CalculateDelay_with_too_small_XRateLimitReset()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = new HttpResponseMessage((HttpStatusCode)428);
			response.Headers.Add("X-RateLimit-Reset", "-1");

			// Act
			var result = sendGridRetryStrategy.GetDelay(1, response);

			// Assert
			result.ShouldBe(TimeSpan.FromSeconds(1));
		}

		[Fact]
		public void CalculateDelay_with_reasonable_XRateLimitReset()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = new HttpResponseMessage((HttpStatusCode)428);
			response.Headers.Add("X-RateLimit-Reset", mockSystemClock.Object.UtcNow.AddSeconds(3).ToUnixTime().ToString());

			// Act
			var result = sendGridRetryStrategy.GetDelay(1, response);

			// Assert
			result.ShouldBe(TimeSpan.FromSeconds(3));
		}

		[Fact]
		public void CalculateDelay_with_too_large_XRateLimitReset()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = new HttpResponseMessage((HttpStatusCode)428);
			response.Headers.Add("X-RateLimit-Reset", mockSystemClock.Object.UtcNow.AddHours(1).ToUnixTime().ToString());

			// Act
			var result = sendGridRetryStrategy.GetDelay(1, response);

			// Assert
			result.ShouldBe(TimeSpan.FromSeconds(5));
		}

		[Fact]
		public void Retry_success()
		{
			// Arrange
			var mockUri = Utils.GetSendGridApiUri("testing");
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, mockUri).Respond((HttpStatusCode)429);
			mockHttp.Expect(HttpMethod.Get, mockUri).Respond("application/json", "Success!");

			var client = Utils.GetFluentClient(mockHttp);

			// Act
			var result = client.SendAsync(HttpMethod.Get, "testing").AsString().Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe("Success!");
		}

		[Fact]
		public void Retry_failure()
		{
			// Arrange
			var mockUri = Utils.GetSendGridApiUri("testing");
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, mockUri).Respond((HttpStatusCode)429);
			mockHttp.Expect(HttpMethod.Get, mockUri).Respond((HttpStatusCode)429);
			mockHttp.Expect(HttpMethod.Get, mockUri).Respond((HttpStatusCode)429);
			mockHttp.Expect(HttpMethod.Get, mockUri).Respond((HttpStatusCode)429);
			mockHttp.Expect(HttpMethod.Get, mockUri).Respond((HttpStatusCode)429);

			var client = Utils.GetFluentClient(mockHttp);

			// Act
			Should.ThrowAsync<Exception>(() => client.SendAsync(HttpMethod.Get, "testing").AsResponse())
				.Result.Message.ShouldBe("The HTTP request failed, and the retry coordinator gave up after the maximum 5 retries");

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
