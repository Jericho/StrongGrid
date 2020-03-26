using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class CustomFieldsConverterTests
	{
		[Fact]
		public void Properties()
		{
			// Act
			var converter = new CustomFieldsConverter();

			// Assert
			converter.CanRead.ShouldBeTrue();
			converter.CanWrite.ShouldBeTrue();
		}

		[Fact]
		public void CanConvert()
		{
			// Act
			var converter = new CustomFieldsConverter();

			// Assert
			converter.CanConvert(null).ShouldBeTrue();
		}

		[Fact]
		public void Write()
		{
			// Arrange
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			var writer = new JsonTextWriter(sw);

			var value = new Field[]
			{
				new Field<string>() { Id = "a", Name = "field1", Value = "111111" },
				new Field<long>() { Id = "b", Name = "field2", Value = 222222 },
				new Field<DateTime>() { Id = "c", Name = "field3", Value = new DateTime(2020, 2, 7, 14, 56, 0, DateTimeKind.Utc) },
			};
			var serializer = new JsonSerializer();

			var converter = new CustomFieldsConverter();

			// Act
			converter.WriteJson(writer, value, serializer);
			var result = sb.ToString();

			// Assert
			result.ShouldBe("{\"a\":\"111111\",\"b\":222222,\"c\":\"2020-02-07T14:56:00.0000000Z\"}");
		}

		[Fact]
		public void Read_invalid()
		{
			// Arrange
			var json = @"[
				{ 'first': 'this first JSON is invalid for this converter' },
				{ 'second': 'this second JSON is also invalid for this converter' }
			]";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new CustomFieldsConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<Field[]>();

			var resultAsArray = (Field[])result;
			resultAsArray.Length.ShouldBe(0);
		}

		[Fact]
		public void Read_multiple()
		{
			// Arrange
			var json = @"{
				'a': '2020-02-07T14:51:00Z',
				'b': 'abc123',
				'c': 123
			}";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new CustomFieldsConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<Field[]>();

			var resultAsArray = (Field[])result;
			resultAsArray.Length.ShouldBe(3);

			resultAsArray[0].Name.ShouldBe("a");
			resultAsArray[0].ShouldBeOfType<Field<DateTime>>();
			((Field<DateTime>)resultAsArray[0]).Value.ShouldBe(new DateTime(2020, 2, 7, 14, 51, 0, DateTimeKind.Utc));

			resultAsArray[1].Name.ShouldBe("b");
			resultAsArray[1].ShouldBeOfType<Field<string>>();
			((Field<string>)resultAsArray[1]).Value.ShouldBe("abc123");

			resultAsArray[2].Name.ShouldBe("c");
			resultAsArray[2].ShouldBeOfType<Field<long>>();
			((Field<long>)resultAsArray[2]).Value.ShouldBe(123);
		}
	}
}
