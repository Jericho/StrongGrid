using Shouldly;
using StrongGrid.Json;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Json
{
	public class EpochConverterTests
	{
		[Fact]
		public void CanConvert_true()
		{
			// Act
			var converter = new EpochConverter();
			var type = typeof(DateTime);

			// Assert
			converter.CanConvert(type).ShouldBeTrue();
		}

		[Fact]
		public void CanConvert_false()
		{
			// Act
			var converter = new EpochConverter();
			var type = typeof(string);

			// Assert
			converter.CanConvert(type).ShouldBeFalse();
		}

		[Fact]
		public void Write()
		{
			// Arrange
			var value = new DateTime(2017, 3, 28, 14, 30, 0, DateTimeKind.Utc);
			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new EpochConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("1490711400");
		}

		[Fact]
		public void Read()
		{
			// Arrange
			var json = "1490711400";
			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new EpochConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldBe(new DateTime(2017, 3, 28, 14, 30, 0, DateTimeKind.Utc));
		}
	}
}
