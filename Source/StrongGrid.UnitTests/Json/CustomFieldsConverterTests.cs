using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Json
{
	public class CustomFieldsConverterTests
	{
		[Fact]
		public void Write()
		{
			// Arrange
			var value = new Field[]
			{
				new Field<string>() { Id = "a", Name = "field1", Value = "111111" },
				new Field<long>() { Id = "b", Name = "field2", Value = 222222 },
				new Field<DateTime>() { Id = "c", Name = "field3", Value = new DateTime(2020, 2, 7, 14, 56, 0, DateTimeKind.Utc) },
			};

			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new CustomFieldsConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("{\"a\":\"111111\",\"b\":222222,\"c\":\"2020-02-07T14:56:00.0000000Z\"}");
		}

		[Fact]
		public void Read_multiple()
		{
			var json = @"{
				""a"": ""2020-02-07T14:51:00Z"",
				""b"": ""abc123"",
				""c"": 123
			}";
			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new CustomFieldsConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<Field[]>();

			var resultAsArray = (Field[])result;
			resultAsArray.Length.ShouldBe(3);

			resultAsArray[0].Name.ShouldBe("a");
			resultAsArray[0].ShouldBeOfType<Field<DateTime>>();
			((Field<DateTime>)resultAsArray[0]).Value.ToUniversalTime().ShouldBe(new DateTime(2020, 2, 7, 14, 51, 0, DateTimeKind.Utc));

			resultAsArray[1].Name.ShouldBe("b");
			resultAsArray[1].ShouldBeOfType<Field<string>>();
			((Field<string>)resultAsArray[1]).Value.ShouldBe("abc123");

			resultAsArray[2].Name.ShouldBe("c");
			resultAsArray[2].ShouldBeOfType<Field<long>>();
			((Field<long>)resultAsArray[2]).Value.ShouldBe(123);
		}
	}
}
