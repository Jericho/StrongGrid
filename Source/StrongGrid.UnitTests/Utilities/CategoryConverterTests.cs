using Shouldly;
using StrongGrid.Utilities;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class CategoryConverterTests
	{
		[Fact]
		public void Write()
		{
			// Arrange
			var value = new[] { "abc123" };
			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new CategoryConverter();

			// Act
			Should.Throw<NotImplementedException>(() => converter.Write(jsonWriter, value, options));
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
