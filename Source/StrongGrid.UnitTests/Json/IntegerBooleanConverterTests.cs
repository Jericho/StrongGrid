using Shouldly;
using StrongGrid.Json;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Json
{
	public class IntegerBooleanConverterTests
	{
		[Fact]
		public void CanConvert_true()
		{
			// Act
			var converter = new IntegerBooleanConverter();
			var type = typeof(bool);

			// Assert
			converter.CanConvert(type).ShouldBeTrue();
		}

		[Fact]
		public void CanConvert_false()
		{
			// Act
			var converter = new IntegerBooleanConverter();
			var type = typeof(string);

			// Assert
			converter.CanConvert(type).ShouldBeFalse();
		}

		[Fact]
		public void Write_true()
		{
			// Arrange
			var value = true;
			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new IntegerBooleanConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("1");
		}

		[Fact]
		public void Write_false()
		{
			// Arrange
			var value = false;
			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new IntegerBooleanConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("0");
		}

		[Fact]
		public void Read_true()
		{
			// Arrange
			var json = "1";
			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new IntegerBooleanConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void Read_false()
		{
			// Arrange
			var json = "\"Anything other than the numeral 1 should yield false\"";
			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new IntegerBooleanConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldBeFalse();
		}
	}
}
