using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shouldly;
using StrongGrid.Models;
using System.IO;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class StringEnumConverterTests
	{
		[Fact]
		public void Read_single()
		{
			// Arrange
			var json = "'in progress'";
			var textReader = new StringReader(json);
			var jsonReader = new JsonTextReader(textReader);
			var objectType = typeof(CampaignStatus);
			var converter = new StringEnumConverter();

			// Act
			jsonReader.Read();
			var result = converter.ReadJson(jsonReader, objectType, (object)null, new JsonSerializer());

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBeOfType<CampaignStatus>();
			((CampaignStatus)result).ShouldBe(CampaignStatus.InProgress);
		}
	}
}
