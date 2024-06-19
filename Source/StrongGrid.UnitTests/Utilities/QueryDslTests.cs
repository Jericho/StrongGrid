using Shouldly;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using System;
using System.Collections.Generic;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class QueryDsl
	{
		public class Version2Tests
		{
			[Fact]
			public void One_condition_with_two_criteria()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaEqual(ContactsFilterField.FirstName, "John"),
						new SearchCriteriaEqual(ContactsFilterField.LastName, "Doe")
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE c.first_name='John' AND c.last_name='Doe'");
			}

			[Fact]
			public void All_contacts()
			{
				// Arrange
				var filter = Array.Empty<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>>();

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c");
			}

			[Fact]
			public void All_contacts_with_firstname_Dave()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaEqual(ContactsFilterField.FirstName, "Dave")
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE c.first_name='Dave'");
			}

			[Fact]
			public void All_contacts_in_Colorado()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaEqual(ContactsFilterField.StateOrProvice, "CO")
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE c.state_province_region='CO'");
			}

			[Fact]
			public void All_contacts_at_gmail()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaLike(ContactsFilterField.EmailAddress, "%gmail.com%")
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE c.email LIKE '%gmail.com%'");
			}

			[Fact]
			public void All_contacts_with_custom_text_field()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaEqual(FilterTable.Contacts, "my_text_custom_field", "abc")
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE c.my_text_custom_field='abc'");
			}

			[Fact]
			public void All_contacts_with_custom_number_field()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaEqual(FilterTable.Contacts, "my_number_custom_field", 12)
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE c.my_number_custom_field=12");
			}

			[Fact]
			public void All_contacts_with_custom_date_field()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaEqual(FilterTable.Contacts, "my_date_custom_field", new DateTime(2021, 1, 1, 12, 46, 24, DateTimeKind.Utc))
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE c.my_date_custom_field='2021-01-01T12:46:24Z'");
			}

			[Fact]
			public void All_contacts_where_alternate_email_contains()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaContains(ContactsFilterField.AlternateEmails, "alternate@gmail.com")
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE array_contains(c.alternate_emails,'alternate@gmail.com')");
			}

			[Fact]
			public void All_contacts_member_of_either_of_lists()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaContains(ContactsFilterField.ListIds, new[] { "aaa", "bbb" })
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE array_contains(c.list_ids,['aaa','bbb'])");
			}

			[Fact]
			public void All_contacts_not_Dave_or_first_name_is_null()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.Or, new[]
					{
						(ISearchCriteria)new SearchCriteriaNotEqual(ContactsFilterField.FirstName, "Dave"),
						(ISearchCriteria)new SearchCriteriaIsNull(ContactsFilterField.FirstName)
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE c.first_name!='Dave' OR c.first_name IS NULL");
			}

			[Fact]
			public void All_contacts_modified_before_given_date()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaLessThan(ContactsFilterField.ModifiedOn, new DateTime(2024, 6, 19, 0, 0, 0, DateTimeKind.Utc))
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion2(filter);

				// Assert
				result.ShouldBe("SELECT c.contact_id, c.updated_at FROM contact_data AS c WHERE c.updated_at<'2024-06-19T00:00:00Z'");
			}
		}

		public class Version1Tests
		{
			[Fact]
			public void Filter_by_subject()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaEqual(EmailActivitiesFilterField.Subject, "This is a subject test"),
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion1(filter);

				// Assert
				result.ShouldBe("subject=\"This is a subject test\"");
			}

			[Fact]
			public void Filter_by_recipient_email()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaEqual(EmailActivitiesFilterField.To, "example@example.com"),
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion1(filter);

				// Assert
				result.ShouldBe("to_email=\"example@example.com\"");
			}

			[Fact]
			public void Filter_by_bounced_email()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaEqual(EmailActivitiesFilterField.ActivityType, EventType.Bounce),
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion1(filter);

				// Assert
				result.ShouldBe("status=\"bounce\"");
			}

			[Fact]
			public void Contacts_modified_prior_to_given_date()
			{
				// Arrange
				var filter = new[]
				{
					new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
					{
						new SearchCriteriaLessThan(ContactsFilterField.ModifiedOn, new DateTime(2024, 6, 19, 0, 0, 0, DateTimeKind.Utc)),
					})
				};

				// Act
				var result = StrongGrid.Utilities.Utils.ToQueryDslVersion1(filter);

				// Assert
				result.ShouldBe("updated_at<TIMESTAMP \"2024-06-19T00:00:00Z\"");
			}
		}
	}
}
