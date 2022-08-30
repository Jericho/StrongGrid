namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be less than a value.
	/// </summary>
	public class SearchCriteriaLessThan : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaLessThan"/> class.
		/// </summary>
		/// <param name="filterTable">The filter table.</param>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaLessThan(FilterTable filterTable, string filterField, object filterValue)
			: base(filterTable, filterField, SearchComparisonOperator.LessThan, filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaLessThan"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaLessThan(ContactsFilterField filterField, object filterValue)
			: this(FilterTable.Contacts, filterField.ToEnumString(), filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaLessThan"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaLessThan(EmailActivitiesFilterField filterField, object filterValue)
			: this(FilterTable.EmailActivities, filterField.ToEnumString(), filterValue)
		{
		}
	}
}
