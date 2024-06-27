using Shouldly;
using StrongGrid.Models.Legacy;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

		public class GetEncoding
		{
			[Fact]
			public void Returns_actual_encoding()
			{
				// Arrange
				var defaultEncoding = Encoding.UTF32;
				var desiredEncoding = Encoding.ASCII;
				var content = new StringContent("This is a test", desiredEncoding);

				// Act
				var result = content.GetEncoding(defaultEncoding);

				// Assert
				result.ShouldBe(desiredEncoding);
			}

			[Fact]
			public void Returns_default_when_charset_is_empty()
			{
				// Arrange
				var defaultEncoding = Encoding.UTF32;
				var desiredEncoding = Encoding.ASCII;
				var content = new StringContent("This is a test");
				content.Headers.ContentType = new MediaTypeHeaderValue("text/plain")
				{
					CharSet = string.Empty
				};

				// Act
				var result = content.GetEncoding(defaultEncoding);

				// Assert
				result.ShouldBe(defaultEncoding);
			}

			[Fact]
			public void Returns_default_when_charset_is_invalid()
			{
				// Arrange
				var defaultEncoding = Encoding.UTF32;
				var desiredEncoding = Encoding.ASCII;
				var content = new StringContent("This is a test");
				content.Headers.ContentType = new MediaTypeHeaderValue("text/plain")
				{
					CharSet = "this is not a valid charset"
				};

				// Act
				var result = content.GetEncoding(defaultEncoding);

				// Assert
				result.ShouldBe(defaultEncoding);
			}
		}

		public class ToDurationString
		{
			[Fact]
			public void Less_than_one_millisecond()
			{
				// Arrange
				var days = 0;
				var hours = 0;
				var minutes = 0;
				var seconds = 0;
				var milliseconds = 0;
				var timespan = new TimeSpan(days, hours, minutes, seconds, milliseconds);

				// Act
				var result = timespan.ToDurationString();

				// Assert
				result.ShouldBe("1 millisecond");
			}

			[Fact]
			public void Normal()
			{
				// Arrange
				var days = 1;
				var hours = 2;
				var minutes = 3;
				var seconds = 4;
				var milliseconds = 5;
				var timespan = new TimeSpan(days, hours, minutes, seconds, milliseconds);

				// Act
				var result = timespan.ToDurationString();

				// Assert
				result.ShouldBe("1 day 2 hours 3 minutes 4 seconds 5 milliseconds");
			}
		}

		public class EnsureStartsWith
		{
			[Fact]
			public void When_string_is_null()
			{
				// Arrange
				var input = (string)null;
				var prefix = "Hello";
				var desired = "Hello";

				// Act
				var result = input.EnsureStartsWith(prefix);

				// Assert
				result.ShouldBe(desired);
			}

			[Fact]
			public void When_string_starts_with_prefix()
			{
				// Arrange
				var input = "Hello world";
				var prefix = "Hello";
				var desired = "Hello world";

				// Act
				var result = input.EnsureStartsWith(prefix);

				// Assert
				result.ShouldBe(desired);
			}

			[Fact]
			public void When_string_does_not_start_with_prefix()
			{
				// Arrange
				var input = "world";
				var prefix = "Hello";
				var desired = "Helloworld";

				// Act
				var result = input.EnsureStartsWith(prefix);

				// Assert
				result.ShouldBe(desired);
			}
		}

		public class EnsureEndsWith
		{
			[Fact]
			public void When_string_is_null()
			{
				// Arrange
				var input = (string)null;
				var prefix = "Hello";
				var desired = "Hello";

				// Act
				var result = input.EnsureEndsWith(prefix);

				// Assert
				result.ShouldBe(desired);
			}

			[Fact]
			public void When_string_ends_with_suffix()
			{
				// Arrange
				var input = "Hello world";
				var suffix = "world";
				var desired = "Hello world";

				// Act
				var result = input.EnsureEndsWith(suffix);

				// Assert
				result.ShouldBe(desired);
			}

			[Fact]
			public void When_string_does_not_end_with_suffix()
			{
				// Arrange
				var input = "Hello";
				var suffix = "world";
				var desired = "Helloworld";

				// Act
				var result = input.EnsureEndsWith(suffix);

				// Assert
				result.ShouldBe(desired);
			}
		}

		public class GetProperty
		{
			[Fact]
			public void When_property_exists()
			{
				// Arrange
				var jsonString = @"{""Name"":""John""}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				var result = jsonObj.GetProperty("Name", true);

				// Assert
				result.ShouldNotBeNull();
				result.Value.ValueEquals("John");
			}

			[Fact]
			public void When_property_does_not_exist_and_throwIfMissing_is_false()
			{
				// Arrange
				var jsonString = @"{""Name"":""John""}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				var result = jsonObj.GetProperty("xxxyyyzzz", false);

				// Assert
				result.ShouldBeNull();
			}

			[Fact]
			public void When_property_does_not_exist_and_throwIfMissing_is_true()
			{
				// Arrange
				var jsonString = @"{""Name"":""John""}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				Should.Throw<Exception>(() => jsonObj.GetProperty("xxxyyyzzz", true));
			}

			[Fact]
			public void When_multilevel_property_exists()
			{
				// Arrange
				var jsonString = @"{""Name"":""John"", ""Child"":{""Name"":""Bob""}}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				var result = jsonObj.GetProperty("Child/Name", true);

				// Assert
				result.ShouldNotBeNull();
				result.Value.ValueEquals("Bob");
			}

			[Fact]
			public void When_multilevel_property_does_not_exist_and_throwIfMissing_is_false()
			{
				// Arrange
				var jsonString = @"{""Name"":""John"", ""Child"":{""Name"":""Bob""}}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				var result = jsonObj.GetProperty("Child/xxxyyyzzz", false);

				// Assert
				result.ShouldBeNull();
			}

			[Fact]
			public void When_multilevel_property_does_not_exist_and_throwIfMissing_is_true()
			{
				// Arrange
				var jsonString = @"{""Name"":""John"", ""Child"":{""Name"":""Bob""}}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				Should.Throw<Exception>(() => jsonObj.GetProperty("Child/xxxyyyzzz", true));
			}
		}

		public class GetPropertyValue
		{
			[Fact]
			public void When_property_exists()
			{
				// Arrange
				var jsonString = @"{""Name"":""John""}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				var result = jsonObj.GetPropertyValue<string>("Name");

				// Assert
				result.ShouldBe("John");
			}

			[Fact]
			public void When_property_does_not_exist()
			{
				// Arrange
				var jsonString = @"{""Name"":""John""}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				var result = jsonObj.GetPropertyValue<string>("xxxyyyzzz");

				// Assert
				result.ShouldBeNull();
			}

			[Fact]
			public void Multiple_properties_exist()
			{
				// Arrange
				var jsonString = @"{""Name"":""John"",""City"":""Atlanta""}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				var result = jsonObj.GetPropertyValue<string>(new[] { "Name", "City" });

				// Assert
				result.ShouldBe("John");
			}

			[Fact]
			public void Multiple_properties_only_one_exists()
			{
				// Arrange
				var jsonString = @"{""Name"":""John"",""City"":""Atlanta""}";

				var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
				var jsonObj = JsonElement.ParseValue(ref reader);

				// Act
				var result = jsonObj.GetPropertyValue<string>(new[] { "xxxyyyzzz", "City" });

				// Assert
				result.ShouldBe("Atlanta");
			}
		}
	}
}
