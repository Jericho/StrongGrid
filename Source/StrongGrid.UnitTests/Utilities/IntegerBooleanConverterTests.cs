using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class IntegerBooleanConverterTests
	{
		[Fact]
		public void Properties()
		{
			// Act
			var converter = new IntegerBooleanConverter();

			// Assert
			converter.CanRead.ShouldBeTrue();
			converter.CanWrite.ShouldBeTrue();
		}

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
		public void Write_null()
		{
			// Arrange
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			var writer = new JsonTextWriter(sw);

			var value = (bool?)null;
			var serializer = new JsonSerializer();

			var converter = new IntegerBooleanConverter();

			// Act
			converter.WriteJson(writer, value, serializer);
			var result = sb.ToString();

			// Assert
			result.ShouldBeEmpty();
		}

		[Fact]
		public void Write_true()
		{
			// Arrange
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			var writer = new JsonTextWriter(sw);

			var value = true;
			var serializer = new JsonSerializer();

			var converter = new IntegerBooleanConverter();

			// Act
			converter.WriteJson(writer, value, serializer);
			var result = sb.ToString();

			// Assert
			result.ShouldBe("1");
		}

		[Fact]
		public void Write_false()
		{
			// Arrange
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			var writer = new JsonTextWriter(sw);

			var value = false;
			var serializer = new JsonSerializer();

			var converter = new IntegerBooleanConverter();

			// Act
			converter.WriteJson(writer, value, serializer);
			var result = sb.ToString();

			// Assert
			result.ShouldBe("0");
		}

		[Fact]
		public void Read_null()
		{
			// Arrange
			var json = "";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new IntegerBooleanConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldBeNull();
		}

		[Fact]
		public void Read_true()
		{
			// Arrange
			var json = "1";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new IntegerBooleanConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldBeOfType<bool>();
			((bool)result).ShouldBeTrue();
		}

		[Fact]
		public void Read_false()
		{
			// Arrange
			var json = "\"Anything other than the numeral 1 should yield false\"";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new IntegerBooleanConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldBeOfType<bool>();
			((bool)result).ShouldBeFalse();
		}
	}
}
