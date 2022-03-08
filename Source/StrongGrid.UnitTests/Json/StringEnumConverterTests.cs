using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models.Legacy;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Json
{
	public class StringEnumConverterTests
	{
		[Theory]
		[InlineData("in progress", CampaignStatus.InProgress)]
		[InlineData("IN PROGRESS", CampaignStatus.InProgress)]
		[InlineData("In Progress", CampaignStatus.InProgress)]
		[InlineData("In progress", CampaignStatus.InProgress)]
		public void Case_insensitive(string json, CampaignStatus expectedStatus)
		{
			// Arrange
			var value = $"\"{json}\"";

			// Act
			var result = JsonSerializer.Deserialize<CampaignStatus>(value, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldBe(expectedStatus);
		}
	}
}
