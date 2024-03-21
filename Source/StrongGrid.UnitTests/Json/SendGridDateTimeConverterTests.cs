using Shouldly;
using StrongGrid.Json;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Json
{
	public class SendGridDateTimeConverterTests
	{
		[Fact]
		public void Write()
		{
			// Arrange
			var value = new DateTime(2017, 3, 28, 16, 19, 0, DateTimeKind.Utc);

			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new DateTimeConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("\"2017-03-28 16:19:00\"");
		}

		[Theory]
		[InlineData("2017-03-28", 2017, 3, 28, 0, 0, 0, 0)]
		[InlineData("2017-03-28 16:19:00", 2017, 3, 28, 16, 19, 0, 0)]
		[InlineData("2017-03-28T16:19:00Z", 2017, 3, 28, 16, 19, 0, 0)]
		[InlineData("2017-03-28T16:19:00.234Z", 2017, 3, 28, 16, 19, 0, 234)]
		[InlineData("2024-03-21 16:21:30 +0000 UTC", 2024, 3, 21, 16, 21, 30, 0)]
		public void Read(string dateAsString, int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			// Arrange
			var json = $"\"{dateAsString}\"";
			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new DateTimeConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldBe(new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc));
		}
	}
}
