using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace StrongGrid.UnitTests
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
				new Field<string>() { Id = 1, Name = "field1", Value = "111111" },
				new Field<long>() { Id = 2, Name = "field2", Value = 222222 },
				new Field<long?>() { Id = 3, Name = "field3", Value = null },
				new Field<DateTime>() { Id = 4, Name = "field4", Value = new DateTime(2017, 3, 28, 13, 55, 0) },
				new Field<DateTime?>() { Id = 5, Name = "field5", Value = null }
			};
			var serializer = new JsonSerializer();

			var converter = new CustomFieldsConverter();

			// Act
			converter.WriteJson(writer, value, serializer);
			var result = sb.ToString();

			// Assert
			result.ShouldBe("[{\"value\":\"111111\",\"id\":1,\"name\":\"field1\"},{\"value\":222222,\"id\":2,\"name\":\"field2\"},{\"id\":3,\"name\":\"field3\"},{\"value\":\"2017-03-28T13:55:00\",\"id\":4,\"name\":\"field4\"},{\"id\":5,\"name\":\"field5\"}]");
		}

		[Fact]
		public void Read_invalid()
		{
			// Arrange
			var json = "{ 'name': 'this JSON is invalid for this converter' }";

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
		public void Read_unknown_fieldtype()
		{
			// Arrange
			var json = @"[
				{ 'id':0, 'name':'field0', 'type':'__bogus__' }
			]";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new CustomFieldsConverter();

			// Act
			jsonReader.Read();
			Should.Throw<Exception>(() => converter.ReadJson(jsonReader, objectType, existingValue, serializer))
				.Message.ShouldBe("__bogus__ is an unknown field type");
		}

		[Fact]
		public void Read_multiple()
		{
			// Arrange
			var json = @"[
				{ 'id':0, 'name':'field0', 'type':'date', 'value':1490709300 },
				{ 'id':1, 'name':'field1', 'type':'date' },
				{ 'id':2, 'name':'field2', 'type':'text', 'value':'abc123' },
				{ 'id':3, 'name':'field3', 'type':'text' },
				{ 'id':4, 'name':'field4', 'type':'number', 'value':123 },
				{ 'id':5, 'name':'field5', 'type':'number' }
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
			resultAsArray.Length.ShouldBe(6);

			resultAsArray[0].Id.ShouldBe(0);
			resultAsArray[0].Name.ShouldBe("field0");
			resultAsArray[0].ShouldBeOfType<Field<DateTime>>();
			((Field<DateTime>)resultAsArray[0]).Value.ShouldBe(new DateTime(2017, 3, 28, 13, 55, 0, DateTimeKind.Utc));

			resultAsArray[1].Id.ShouldBe(1);
			resultAsArray[1].Name.ShouldBe("field1");
			resultAsArray[1].ShouldBeOfType<Field<DateTime?>>();
			((Field<DateTime?>)resultAsArray[1]).Value.ShouldBeNull();

			resultAsArray[2].Id.ShouldBe(2);
			resultAsArray[2].Name.ShouldBe("field2");
			resultAsArray[2].ShouldBeOfType<Field<string>>();
			((Field<string>)resultAsArray[2]).Value.ShouldBe("abc123");

			resultAsArray[3].Id.ShouldBe(3);
			resultAsArray[3].Name.ShouldBe("field3");
			resultAsArray[3].ShouldBeOfType<Field<string>>();
			((Field<string>)resultAsArray[3]).Value.ShouldBeNull();

			resultAsArray[4].Id.ShouldBe(4);
			resultAsArray[4].Name.ShouldBe("field4");
			resultAsArray[4].ShouldBeOfType<Field<long>>();
			((Field<long>)resultAsArray[4]).Value.ShouldBe(123);

			resultAsArray[5].Id.ShouldBe(5);
			resultAsArray[5].Name.ShouldBe("field5");
			resultAsArray[5].ShouldBeOfType<Field<long?>>();
			((Field<long?>)resultAsArray[5]).Value.ShouldBeNull();
		}
	}
}
