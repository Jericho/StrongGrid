using Shouldly;
using StrongGrid.Json;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Json
{
	public class CategoryConverterTests
	{
		[Fact]
		public void Write()
		{
			// Arrange
			var value = new[] { "category1", "category2", "category3" };
			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new CategoryConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("[\"category1\",\"category2\",\"category3\"]");
		}

		[Fact]
		public void Read_single()
		{
			// Arrange
			var json = "\"category1\"";
			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new CategoryConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].ShouldBe("category1");
		}

		[Fact]
		public void Read_multiple()
		{
			// Arrange
			var json = "[\"category1\",\"category2\",\"category3\"]";

			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new CategoryConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
			result[0].ShouldBe("category1");
			result[1].ShouldBe("category2");
			result[2].ShouldBe("category3");
		}
	}
}
