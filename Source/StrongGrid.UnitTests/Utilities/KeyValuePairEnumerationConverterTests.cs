using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class KeyValuePairEnumerationConverterTests
	{
		[Fact]
		public void Properties()
		{
			// Act
			var converter = new KeyValuePairEnumerationConverter();

			// Assert
			converter.CanRead.ShouldBeTrue();
			converter.CanWrite.ShouldBeTrue();
		}

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
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			var writer = new JsonTextWriter(sw);

			var value = (object)null;
			var serializer = new JsonSerializer();

			var converter = new KeyValuePairEnumerationConverter();

			// Act
			converter.WriteJson(writer, value, serializer);
			var result = sb.ToString();

			// Assert
			result.ShouldBeEmpty();
		}

		[Fact]
		public void Write()
		{
			// Arrange
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			var writer = new JsonTextWriter(sw);

			var value = new[]
			{
				new KeyValuePair<string, string>("key1", "value1"),
				new KeyValuePair<string, string>("key2", "value2")
			};
			var serializer = new JsonSerializer();

			var converter = new KeyValuePairEnumerationConverter();

			// Act
			converter.WriteJson(writer, value, serializer);
			var result = sb.ToString();

			// Assert
			result.ShouldBe("{\"key1\":\"value1\",\"key2\":\"value2\"}");
		}

		[Fact]
		public void Read()
		{
			// Arrange
			var json = "{ \"some_value\": \"QWERTY\", \"another_value\": \"ABC_123\" }";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = typeof(KeyValuePair<string, string>[]);
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new KeyValuePairEnumerationConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

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
