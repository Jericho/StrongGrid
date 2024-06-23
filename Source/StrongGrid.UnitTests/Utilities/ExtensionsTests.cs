using Shouldly;
using StrongGrid.Models.Legacy;
using System;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class Extensions
	{
		public class FromUnixTime
		{
			[Fact]
			public void Zero_returns_EPOCH()
			{
				// Arrange
				var unixTime = 0L;

				// Act
				var result = unixTime.FromUnixTime();

				// Assert
				result.ShouldBe(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			}

			[Fact]
			public void Seconds()
			{
				// Arrange
				var unixTime = 1468155111L;

				// Act
				var result = unixTime.FromUnixTime(Internal.UnixTimePrecision.Seconds);

				// Assert
				result.ShouldBe(new DateTime(2016, 7, 10, 12, 51, 51, DateTimeKind.Utc));
			}

			[Fact]
			public void Milliseconds()
			{
				// Arrange
				var unixTime = 1719149961333L;

				// Act
				var result = unixTime.FromUnixTime(Internal.UnixTimePrecision.Milliseconds);

				// Assert
				result.ShouldBe(new DateTime(2024, 6, 23, 13, 39, 21, 333, DateTimeKind.Utc));
			}

			[Fact]
			public void Throws_when_unknown_precision()
			{
				// Arrange
				var unixTime = 1468155111L;
				var unknownPrecision = (Internal.UnixTimePrecision)123;

				// Act
				Should.Throw<Exception>(() => unixTime.FromUnixTime(unknownPrecision));
			}
		}

		public class ToUnixTime
		{
			[Fact]
			public void EPOCH_returns_zero()
			{
				// Arrange
				var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

				// Act
				var result = date.ToUnixTime();

				// Assert
				result.ShouldBe(0);
			}

			[Fact]
			public void Seconds()
			{
				// Arrange
				var date = new DateTime(2016, 7, 10, 12, 51, 51, DateTimeKind.Utc);

				// Act
				var result = date.ToUnixTime(Internal.UnixTimePrecision.Seconds);

				// Assert
				result.ShouldBe(1468155111);
			}

			[Fact]
			public void Milliseconds()
			{
				// Arrange
				var date = new DateTime(2024, 6, 23, 13, 39, 21, 333, DateTimeKind.Utc);

				// Act
				var result = date.ToUnixTime(Internal.UnixTimePrecision.Milliseconds);

				// Assert
				result.ShouldBe(1719149961333);
			}

			[Fact]
			public void Throws_when_unknown_precision()
			{
				// Arrange
				var date = new DateTime(2024, 6, 23, 13, 39, 21, 333, DateTimeKind.Utc);
				var unknownPrecision = (Internal.UnixTimePrecision)123;

				// Act
				Should.Throw<Exception>(() => date.ToUnixTime(unknownPrecision));
			}
		}

		public class ToEnum
		{
			[Fact]
			public void Throws_when_invalid_value()
			{
				Should.Throw<Exception>(() => "This is not a valid value".ToEnum<CampaignStatus>());
			}

			[Theory]
			[InlineData("in progress", CampaignStatus.InProgress)]
			[InlineData("IN PROGRESS", CampaignStatus.InProgress)]
			[InlineData("In Progress", CampaignStatus.InProgress)]
			[InlineData("In progress", CampaignStatus.InProgress)]
			public void Is_case_insensitive(string descripion, CampaignStatus expectedStatus)
			{
				// Act
				var result = descripion.ToEnum<CampaignStatus>();

				// Assert
				result.ShouldBe(expectedStatus);
			}
		}
	}
}
