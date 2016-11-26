using Shouldly;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class ExtensionsTests
	{
		[Fact]
		public void FromUnixTime_EPOCH()
		{
			// Arrange
			var unixTime = 0L;

			// Act
			var result = unixTime.FromUnixTime();

			// Assert
			result.ShouldBe(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
		}

		[Fact]
		public void FromUnixTime_2016()
		{
			// Arrange
			var unixTime = 1468155111L;

			// Act
			var result = unixTime.FromUnixTime();

			// Assert
			result.ShouldBe(new DateTime(2016, 7, 10, 12, 51, 51, DateTimeKind.Utc));
		}
		[Fact]

		public void ToUnixTime_EPOCH()
		{
			// Arrange
			var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			// Act
			var result = date.ToUnixTime();

			// Assert
			result.ShouldBe(0);
		}

		[Fact]
		public void ToUnixTime_2016()
		{
			// Arrange
			var date = new DateTime(2016, 7, 10, 12, 51, 51, DateTimeKind.Utc);

			// Act
			var result = date.ToUnixTime();

			// Assert
			result.ShouldBe(1468155111);
		}

		[Fact]
		public void EnsureSuccess_success()
		{
			// Arrange
			var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

			// Act
			httpResponse.EnsureSuccess();

			// Assert
			// Nothing to assert, we just want to make sure no exception was thrown
		}

		[Fact]
		public void EnsureSuccess_failure()
		{
			// Arrange
			var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

			// Act
			Should.Throw<Exception>(() => httpResponse.EnsureSuccess())
				.Message.ShouldBe("StatusCode: ServiceUnavailable");

			// Assert
			// Nothing to assert, we just want to make sure an exception was thrown
		}

		[Fact]
		public void EnsureSuccess_failure_with_content()
		{
			// Arrange
			var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
			httpResponse.Content = new StringContent("Hello World");

			// Act
			Should.Throw<Exception>(() => httpResponse.EnsureSuccess())
				.Message.ShouldBe("Hello World");

			// Assert
			// Nothing to assert, we just want to make sure an exception was thrown
		}
	}
}
