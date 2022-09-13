namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be different than a value.
	/// </summary>
	public class SearchCriteriaNotEqual : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotEqual"/> class.
		/// </summary>
		/// <param name="filterTable">The filter table.</param>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaNotEqual(FilterTable filterTable, string filterField, object filterValue)
			: base(filterTable, filterField, SearchComparisonOperator.NotEqual, filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaNotEqual(ContactsFilterField filterField, object filterValue)
			: this(FilterTable.Contacts, filterField.ToEnumString(), filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaNotEqual(EmailActivitiesFilterField filterField, object filterValue)
			: this(FilterTable.EmailActivities, filterField.ToEnumString(), filterValue)
		{
		}
	}
}
