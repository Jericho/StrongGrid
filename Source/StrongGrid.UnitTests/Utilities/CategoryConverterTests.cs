using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Utilities;
using System;
using System.IO;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class CategoryConverterTests
	{
		[Fact]
		public void Properties()
		{
			// Act
			var converter = new CategoryConverter();

			// Assert
			converter.CanRead.ShouldBeTrue();
			converter.CanWrite.ShouldBeFalse();
		}

		[Fact]
		public void CanConvert()
		{
			// Act
			var converter = new CategoryConverter();

			// Assert
			converter.CanConvert(null).ShouldBeTrue();
		}

		[Fact]
		public void Write()
		{
			// Arrange
			var writer = (JsonWriter)null;
			var value = (object)null;
			var serializer = (JsonSerializer)null;

			var converter = new CategoryConverter();

			// Act
			Should.Throw<NotImplementedException>(() => converter.WriteJson(writer, value, serializer));
		}

		[Fact]
		public void Read_single()
		{
			// Arrange
			var json = "'category1'";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new CategoryConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<string[]>();

			var resultAsArray = (string[])result;
			resultAsArray.Length.ShouldBe(1);
			resultAsArray[0].ShouldBe("category1");
		}

		[Fact]
		public void Read_multiple()
		{
			// Arrange
			var json = "['category1','category2','category3']";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new CategoryConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<string[]>();

			var resultAsArray = (string[])result;
			resultAsArray.Length.ShouldBe(3);
			resultAsArray[0].ShouldBe("category1");
			resultAsArray[1].ShouldBe("category2");
			resultAsArray[2].ShouldBe("category3");
		}
	}
}
