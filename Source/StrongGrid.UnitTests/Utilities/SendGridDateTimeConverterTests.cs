using Shouldly;
using StrongGrid.Utilities;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class SendGridDateTimeConverterTests
	{
		[Fact]
		public void Write()
		{
			// Arrange
			var value = new DateTime(2017, 3, 28, 16, 19, 0, DateTimeKind.Utc);

			var ms = new MemoryStream();
			var jsonWriter = new Utf8JsonWriter(ms);
			var options = new JsonSerializerOptions();

			var converter = new SendGridDateTimeConverter();

			// Act
			converter.Write(jsonWriter, value, options);
			jsonWriter.Flush();

			ms.Position = 0;
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();

			// Assert
			result.ShouldBe("\"2017-03-28 16:19:00\"");
		}

		[Fact]
		public void Read()
		{
			// Arrange
			var json = "\"2017-03-28 16:19:00\"";

			var jsonUtf8 = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(json);
			var jsonReader = new Utf8JsonReader(jsonUtf8);
			var objectType = (Type)null;
			var options = new JsonSerializerOptions();

			var converter = new SendGridDateTimeConverter();

			// Act
			jsonReader.Read();
			var result = converter.Read(ref jsonReader, objectType, options);

			// Assert
			result.ShouldBe(new DateTime(2017, 3, 28, 16, 19, 0, DateTimeKind.Utc));
		}
	}
}
