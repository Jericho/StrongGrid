using Shouldly;
using StrongGrid.Models.Search;
using Xunit;

namespace StrongGrid.UnitTests.Models.Search
{
	public class SearchCriteriaTests
	{
		[Fact]
		public void SearchCriteriaBetween()
		{
			// Arrange
			var searchCriteria = new SearchCriteriaBetween(ContactsFilterField.FirstName, "Allen", "Bob");

			// Act
			var result = searchCriteria.ToString();

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBe("first_name BETWEEN \"Allen\" AND \"Bob\"");
		}

		[Fact]
		public void SearchCriteriaEqual()
		{
			// Arrange
			var searchCriteria = new SearchCriteriaEqual(ContactsFilterField.LastName, "Smith");

			// Act
			var result = searchCriteria.ToString();

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBe("last_name=\"Smith\"");
		}

		[Fact]
		public void SearchCriteriaContains()
		{
			// Arrange
			var searchCriteria = new SearchCriteriaContains(ContactsFilterField.ListIds, "my-list-id");

			// Act
			var result = searchCriteria.ToString();

			// Assert
			result.ShouldNotBeNull();
			result.ShouldBe("CONTAINS(list_ids,\"my-list-id\")");
		}
	}
}
