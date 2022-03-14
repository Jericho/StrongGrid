using Shouldly;
using StrongGrid.Json;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Json
{
	public class LegacyCustomFieldsConverterTests
	{
		[Theory]
		[InlineData(typeof(StrongGrid.Models.Legacy.Field[]), true)]
		[InlineData((Type)null, false)]
		[InlineData(typeof(TimeSpan), false)]
		[InlineData(typeof(string), false)]
		[InlineData(typeof(StrongGrid.Models.Field), false)]
		public void CanConvert(Type typeToConvert, bool expected)
		{
			// Act
			var converter = new LegacyCustomFieldsConverter();

			// Assert
			converter.CanConvert(typeToConvert).ShouldBe(expected);
		}

		[Fact]
		public void Write()
		{
			// Arrange
			var value = new StrongGrid.Models.Legacy.Field[]
			{
				new StrongGrid.Models.Legacy.Field<string>() { Id = 1, Name = "field1", Value = "111111" },
				new StrongGrid.Models.Legacy.Field<long>() { Id = 2, Name = "field2", Value = 222222 },
				new StrongGrid.Models.Legacy.Field<long?>() { Id = 3, Name = "field3", Value = null },
				new StrongGrid.Models.Legacy.Field<DateTime>() { Id = 4, Name = "field4", Value = new DateTime(2017, 3, 28, 13, 55, 0) },
				new StrongGrid.Models.Legacy.Field<DateTime?>() { Id = 5, Name = "field5", Value = null }
			};

			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new LegacyCustomFieldsConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("[{\"value\":\"111111\",\"id\":1,\"name\":\"field1\"},{\"value\":222222,\"id\":2,\"name\":\"field2\"},{\"id\":3,\"name\":\"field3\"},{\"value\":\"2017-03-28T13:55:00\",\"id\":4,\"name\":\"field4\"},{\"id\":5,\"name\":\"field5\"}]");
		}

		[Fact]
		public void Read_invalid()
		{
			// Arrange
			var json = "{ \"name\": \"this JSON is invalid for this converter\" }";

			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new LegacyCustomFieldsConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(0);
		}

		[Fact]
		public void Read_unknown_fieldtype()
		{
			// Arrange
			var json = @"[
				{ ""id"":0, ""name"":""field0"", ""type"":""__bogus__"" }
			]";

			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new LegacyCustomFieldsConverter();

			// Act
			jsonReader.Read();

			// This try...catch is a workaround for the fact that we can't use the following code:
			// Should.Throw<Exception>(() => converter.Read(ref jsonReader, objectType, options)).Message.ShouldBe("__bogus__ is an unknown field type");
			// due to the following compile time error:
			// Cannot use ref local 'jsonReader' inside an anonymous method, lambda expression, or query expression
			try
			{
				var fields = converter.Read(ref jsonReader, objectType, options);
			}
			catch (Exception e) when (e.Message == "__bogus__ is an unknown field type")
			{
				// This is the expected exception
			}
		}

		[Fact]
		public void Read_multiple()
		{
			// Arrange
			var json = @"[
				{ ""id"":0, ""name"":""field0"", ""type"":""date"", ""value"":1490709300 },
				{ ""id"":1, ""name"":""field1"", ""type"":""date"" },
				{ ""id"":2, ""name"":""field2"", ""type"":""text"", ""value"":""abc123"" },
				{ ""id"":3, ""name"":""field3"", ""type"":""text"" },
				{ ""id"":4, ""name"":""field4"", ""type"":""number"", ""value"":123 },
				{ ""id"":5, ""name"":""field5"", ""type"":""number"" }
			]";

			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new LegacyCustomFieldsConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<StrongGrid.Models.Legacy.Field[]>();

			var resultAsArray = (StrongGrid.Models.Legacy.Field[])result;
			resultAsArray.Length.ShouldBe(6);

			resultAsArray[0].Id.ShouldBe(0);
			resultAsArray[0].Name.ShouldBe("field0");
			resultAsArray[0].ShouldBeOfType<StrongGrid.Models.Legacy.Field<DateTime>>();
			((StrongGrid.Models.Legacy.Field<DateTime>)resultAsArray[0]).Value.ShouldBe(new DateTime(2017, 3, 28, 13, 55, 0, DateTimeKind.Utc));

			resultAsArray[1].Id.ShouldBe(1);
			resultAsArray[1].Name.ShouldBe("field1");
			resultAsArray[1].ShouldBeOfType<StrongGrid.Models.Legacy.Field<DateTime?>>();
			((StrongGrid.Models.Legacy.Field<DateTime?>)resultAsArray[1]).Value.ShouldBeNull();

			resultAsArray[2].Id.ShouldBe(2);
			resultAsArray[2].Name.ShouldBe("field2");
			resultAsArray[2].ShouldBeOfType<StrongGrid.Models.Legacy.Field<string>>();
			((StrongGrid.Models.Legacy.Field<string>)resultAsArray[2]).Value.ShouldBe("abc123");

			resultAsArray[3].Id.ShouldBe(3);
			resultAsArray[3].Name.ShouldBe("field3");
			resultAsArray[3].ShouldBeOfType<StrongGrid.Models.Legacy.Field<string>>();
			((StrongGrid.Models.Legacy.Field<string>)resultAsArray[3]).Value.ShouldBeNull();

			resultAsArray[4].Id.ShouldBe(4);
			resultAsArray[4].Name.ShouldBe("field4");
			resultAsArray[4].ShouldBeOfType<StrongGrid.Models.Legacy.Field<long>>();
			((StrongGrid.Models.Legacy.Field<long>)resultAsArray[4]).Value.ShouldBe(123);

			resultAsArray[5].Id.ShouldBe(5);
			resultAsArray[5].Name.ShouldBe("field5");
			resultAsArray[5].ShouldBeOfType<StrongGrid.Models.Legacy.Field<long?>>();
			((StrongGrid.Models.Legacy.Field<long?>)resultAsArray[5]).Value.ShouldBeNull();
		}
	}
}
