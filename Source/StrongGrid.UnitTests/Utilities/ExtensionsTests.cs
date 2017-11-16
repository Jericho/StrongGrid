using Shouldly;
using StrongGrid.Utilities;
using System;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
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
	}
}
