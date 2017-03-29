using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class MetricsConverterTests
	{
		[Fact]
		public void Properties()
		{
			// Act
			var converter = new MetricsConverter();

			// Assert
			converter.CanRead.ShouldBeTrue();
			converter.CanWrite.ShouldBeFalse();
		}

		[Fact]
		public void CanConvert()
		{
			// Act
			var converter = new MetricsConverter();
			var type = (Type)null;

			// Assert
			converter.CanConvert(type).ShouldBeTrue();
		}

		[Fact]
		public void Write()
		{
			// Arrange
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			var writer = new JsonTextWriter(sw);

			var value = (object)null;
			var serializer = new JsonSerializer();

			var converter = new MetricsConverter();

			// Act
			Should.Throw<NotImplementedException>(() => converter.WriteJson(writer, value, serializer));
		}

		[Fact]
		public void Read()
		{
			// Arrange
			var json = "{\"metric1\":1,\"metric2\":2}";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new MetricsConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<KeyValuePair<string, long>[]>();

			var resultAsArray = (KeyValuePair<string, long>[])result;
			resultAsArray.Length.ShouldBe(2);
			resultAsArray[0].Key.ShouldBe("metric1");
			resultAsArray[0].Value.ShouldBe(1);
			resultAsArray[1].Key.ShouldBe("metric2");
			resultAsArray[1].Value.ShouldBe(2);
		}
	}
}
