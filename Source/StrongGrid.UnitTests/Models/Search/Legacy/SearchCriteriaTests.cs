using Shouldly;
using StrongGrid.Models.Search.Legacy;
using Xunit;

namespace StrongGrid.UnitTests.Models.Search.Legacy
{
	public class SearchCriteriaTests
	{
		[Fact]
		public void SearchCriteriaBetween()
		{
			// Arrange
			var searchCriteria = new SearchCriteriaBetween(EmailActivitiesFilterField.Clicks, 1, 5);

			// Act
			var result = searchCriteria.ToString();

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBe("clicks BETWEEN 1 AND 5");
		}

		[Fact]
		public void SearchCriteriaIn()
		{
			// Arrange
			var searchCriteria = new SearchCriteriaIn(EmailActivitiesFilterField.Subject, new[] { "Subject1", "Subject2" });

			// Act
			var result = searchCriteria.ToString();

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBe("subject IN (\"Subject1\",\"Subject2\")");
		}

		[Fact]
		public void SearchCriteriaEqual()
		{
			// Arrange
			var searchCriteria = new SearchCriteriaEqual(EmailActivitiesFilterField.CampaignName, "abc123");

			// Act
			var result = searchCriteria.ToString();

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBe("campaign_name=\"abc123\"");
		}
	}
}
