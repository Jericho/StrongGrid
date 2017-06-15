using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class SendGridDateTimeConverterTests
	{
		[Fact]
		public void Properties()
		{
			// Act
			var converter = new SendGridDateTimeConverter();

			// Assert
			converter.CanRead.ShouldBeTrue();
			converter.CanWrite.ShouldBeTrue();
		}

		[Fact]
		public void CanConvert_true()
		{
			// Act
			var converter = new SendGridDateTimeConverter();
			var type = typeof(DateTime);

			// Assert
			converter.CanConvert(type).ShouldBeTrue();
		}

		[Fact]
		public void CanConvert_false()
		{
			// Act
			var converter = new SendGridDateTimeConverter();
			var type = (Type)null;

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

			var value = (object)null;
			var serializer = new JsonSerializer();

			var converter = new SendGridDateTimeConverter();

			// Act
			converter.WriteJson(writer, value, serializer);
			var result = sb.ToString();

			// Assert
			result.ShouldBeEmpty();
		}

		[Fact]
		public void Write()
		{
			// Arrange
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			var writer = new JsonTextWriter(sw);

			var value = new DateTime(2017, 3, 28, 16, 19, 0, DateTimeKind.Utc);
			var serializer = new JsonSerializer();

			var converter = new SendGridDateTimeConverter();

			// Act
			converter.WriteJson(writer, value, serializer);
			var result = sb.ToString();

			// Assert
			result.ShouldBe("\"2017-03-28 16:19:00\"");
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

			var converter = new SendGridDateTimeConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldBeNull();
		}

		[Fact]
		public void Read()
		{
			// Arrange
			var json = "\"2017-03-28 16:19:00\"";

			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = (Type)null;
			var existingValue = (object)null;
			var serializer = new JsonSerializer();

			var converter = new SendGridDateTimeConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, existingValue, serializer);

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<DateTime>();
			((DateTime)result).ShouldBe(new DateTime(2017, 3, 28, 16, 19, 0, DateTimeKind.Utc));
		}
	}
}
