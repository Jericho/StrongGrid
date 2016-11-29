using Moq;
using Shouldly;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class SendGridRetryStrategyTests
	{
		[Fact]
		public void ShouldRetry_returns_false_when_attempts_equal_max()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = (HttpResponseMessage)null;

			// Act
			var result = sendGridRetryStrategy.ShouldRetry(maxAttempts, response);

			// Assert
			result.ShouldBeFalse();
		}

		[Fact]
		public void ShouldRetry_returns_false_when_attempts_exceed_max()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = (HttpResponseMessage)null;

			// Act
			var result = sendGridRetryStrategy.ShouldRetry(maxAttempts + 1, response);

			// Assert
			result.ShouldBeFalse();
		}

		[Fact]
		public void ShouldRetry_returns_false_when_previous_response_is_null()
		{
			// Arrange
			var maxAttempts = 5;
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var sendGridRetryStrategy = new SendGridRetryStrategy(maxAttempts, mockSystemClock.Object);
			var response = (HttpResponseMessage)null;

			// Act
			var result = sendGridRetryStrategy.ShouldRetry(1, response);

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
			var result = sendGridRetryStrategy.ShouldRetry(1, response);

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
			var result = sendGridRetryStrategy.ShouldRetry(1, response);

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
			var result = sendGridRetryStrategy.GetNextDelay(1, response);

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
			var result = sendGridRetryStrategy.GetNextDelay(1, response);

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
			var result = sendGridRetryStrategy.GetNextDelay(1, response);

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
			var result = sendGridRetryStrategy.GetNextDelay(1, response);

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
			var result = sendGridRetryStrategy.GetNextDelay(1, response);

			// Assert
			result.ShouldBe(TimeSpan.FromSeconds(5));
		}
	}
}
