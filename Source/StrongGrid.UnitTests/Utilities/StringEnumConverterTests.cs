using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Models;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
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
			var value = $"'{json}'";

			// Act
			var result = JsonConvert.DeserializeObject<CampaignStatus>(value);

			// Assert
			result.ShouldBe(expectedStatus);
		}
	}
}
