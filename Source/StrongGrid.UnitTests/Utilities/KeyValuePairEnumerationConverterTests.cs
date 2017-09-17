using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class KeyValuePairEnumerationConverterTests
	{
		[Fact]
		public void Properties()
		{
			// Act
			var converter = new KeyValuePairEnumerationConverter();

			// Assert
			converter.CanRead.ShouldBeFalse();
			converter.CanWrite.ShouldBeTrue();
		}

		[Fact]
		public void CanConvert_true()
		{
			// Act
			var converter = new KeyValuePairEnumerationConverter();
			var type = typeof(KeyValuePair<string, string>[]);

			// Assert
			converter.CanConvert(type).ShouldBeTrue();
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
			var json = "";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new KeyValuePairEnumerationConverter();

			// Act
			jsonReader.Read();
			Should.Throw<NotImplementedException>(() => converter.ReadJson(jsonReader, objectType, existingValue, serializer));
		}
	}
}
