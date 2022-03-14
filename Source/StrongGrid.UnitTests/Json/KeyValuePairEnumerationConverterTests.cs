using Shouldly;
using StrongGrid.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Json
{
	public class KeyValuePairEnumerationConverterTests
	{
		[Fact]
		public void CanConvert_true()
		{
			// Act
			var converter = new KeyValuePairEnumerationConverter();
			var objectType = typeof(KeyValuePair<string, string>[]);

			// Assert
			converter.CanConvert(objectType).ShouldBeTrue();
		}

		[Fact]
		public void Write_null()
		{
			// Arrange
			var value = (KeyValuePair<string, string>[])null;
			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new KeyValuePairEnumerationConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBeEmpty();
		}

		[Fact]
		public void Write()
		{
			// Arrange
			var value = new[]
			{
				new KeyValuePair<string, string>("key1", "value1"),
				new KeyValuePair<string, string>("key2", "value2")
			};
			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new KeyValuePairEnumerationConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("{\"key1\":\"value1\",\"key2\":\"value2\"}");
		}

		[Fact]
		public void Read()
		{
			// Arrange
			var json = "{ \"some_value\": \"QWERTY\", \"another_value\": \"ABC_123\" }";
			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new KeyValuePairEnumerationConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<KeyValuePair<string, string>[]>();

			var resultAsArray = (KeyValuePair<string, string>[])result;
			resultAsArray.Length.ShouldBe(2);
			resultAsArray[0].Key.ShouldBe("some_value");
			resultAsArray[0].Value.ShouldBe("QWERTY");
			resultAsArray[1].Key.ShouldBe("another_value");
			resultAsArray[1].Value.ShouldBe("ABC_123");
		}
	}
}
