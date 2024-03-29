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
	public class MetricsConverterTests
	{
		[Theory]
		[InlineData(typeof(KeyValuePair<string, long>[]), true)]
		[InlineData((Type)null, false)]
		[InlineData(typeof(TimeSpan), false)]
		[InlineData(typeof(string), false)]
		[InlineData(typeof(StrongGrid.Models.Field), false)]
		public void CanConvert(Type typeToConvert, bool expected)
		{
			// Arrange
			var converter = new MetricsConverter();

			// Assert
			converter.CanConvert(typeToConvert).ShouldBe(expected);
		}


		[Fact]
		public void Write()
		{
			// Arrange
			var value = new[]
			{
				new KeyValuePair<string, long>("metric1", 1),
				new KeyValuePair<string, long>("metric2", 2)
			};
			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new MetricsConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("{\"metric1\":1,\"metric2\":2}");
		}

		[Fact]
		public void Read()
		{
			// Arrange
			var json = "{\"metric1\":1,\"metric2\":2}";

			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new MetricsConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Key.ShouldBe("metric1");
			result[0].Value.ShouldBe(1);
			result[1].Key.ShouldBe("metric2");
			result[1].Value.ShouldBe(2);
		}
	}
}
