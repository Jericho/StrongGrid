using Shouldly;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using System;
using System.Collections.Generic;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class QueryDslTests
	{
		public class ToContactsQueryDsl
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE contacts.first_name='John' AND contacts.last_name='Doe'");
			}

			[Fact]
			public void All_contacts()
			{
				// Arrange
				var filter = Array.Empty<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>>();

				// Act
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts");
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE contacts.first_name='Dave'");
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE contacts.state_province_region='CO'");
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE contacts.email LIKE '%gmail.com%'");
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE contacts.my_text_custom_field='abc'");
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE contacts.my_number_custom_field=12");
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE contacts.my_date_custom_field='2021-01-01T12:46:24Z'");
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE array_contains(contacts.alternate_emails,'alternate@gmail.com')");
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE array_contains(contacts.list_ids,['aaa','bbb'])");
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
				var result = StrongGrid.Utilities.Utils.ToContactsQueryDsl(filter);

				// Assert
				result.ShouldBe("SELECT contacts.contact_id, contacts.updated_at FROM contact_data AS contacts WHERE contacts.first_name!='Dave' OR contacts.first_name IS NULL");
			}
		}

		public class ToEmailActivitiesQueryDsl
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
				var result = StrongGrid.Utilities.Utils.ToEmailActivitiesQueryDsl(filter);

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
				var result = StrongGrid.Utilities.Utils.ToEmailActivitiesQueryDsl(filter);

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
				var result = StrongGrid.Utilities.Utils.ToEmailActivitiesQueryDsl(filter);

				// Assert
				result.ShouldBe("status=\"bounce\"");
			}
		}
	}
}
