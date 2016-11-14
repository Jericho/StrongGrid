using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrongGrid.Utilities;
using System;
using System.Net.Http.Headers;

namespace StrongGrid.UnitTests
{
	[TestClass]
	public class AsyncDelayerTests
	{
		[TestMethod]
		public void CalculateDelay_with_null_HttpHeaders()
		{
			// Arrange
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var asyncDelayer = new AsyncDelayer(mockSystemClock.Object);
			var headers = (HttpHeaders)null;

			// Act
			var result = asyncDelayer.CalculateDelay(headers);

			// Assert
			Assert.AreEqual(TimeSpan.FromSeconds(1), result);
		}

		[TestMethod]
		public void CalculateDelay_without_XRateLimitReset()
		{
			// Arrange
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var asyncDelayer = new AsyncDelayer(mockSystemClock.Object);
			var headers = new FakeHttpHeaders();

			// Act
			var result = asyncDelayer.CalculateDelay(headers);

			// Assert
			Assert.AreEqual(TimeSpan.FromSeconds(1), result);
		}

		[TestMethod]
		public void CalculateDelay_with_too_small_XRateLimitReset()
		{
			// Arrange
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var asyncDelayer = new AsyncDelayer(mockSystemClock.Object);
			var headers = new FakeHttpHeaders();
			headers.Add("X-RateLimit-Reset", "-1");

			// Act
			var result = asyncDelayer.CalculateDelay(headers);

			// Assert
			Assert.AreEqual(TimeSpan.FromSeconds(1), result);
		}

		[TestMethod]
		public void CalculateDelay_with_reasonable_XRateLimitReset()
		{
			// Arrange
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var asyncDelayer = new AsyncDelayer(mockSystemClock.Object);
			var headers = new FakeHttpHeaders();
			headers.Add("X-RateLimit-Reset", mockSystemClock.Object.UtcNow.AddSeconds(3).ToUnixTime().ToString());

			// Act
			var result = asyncDelayer.CalculateDelay(headers);

			// Assert
			Assert.AreEqual(TimeSpan.FromSeconds(3), result);
		}

		[TestMethod]
		public void CalculateDelay_with_too_large_XRateLimitReset()
		{
			// Arrange
			var mockSystemClock = new MockSystemClock(2016, 11, 11, 13, 14, 0, 0);
			var asyncDelayer = new AsyncDelayer(mockSystemClock.Object);
			var headers = new FakeHttpHeaders();
			headers.Add("X-RateLimit-Reset", mockSystemClock.Object.UtcNow.AddHours(1).ToUnixTime().ToString());

			// Act
			var result = asyncDelayer.CalculateDelay(headers);

			// Assert
			Assert.AreEqual(TimeSpan.FromSeconds(5), result);
		}
	}
}
