namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be equal to a value.
	/// </summary>
	public class SearchCriteriaEqual : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaEqual(string filterField, object filterValue)
			: base(filterField, SearchComparisonOperator.Equal, filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaEqual(ContactsFilterField filterField, object filterValue)
			: this(filterField.ToEnumString(), filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaEqual(EmailActivitiesFilterField filterField, object filterValue)
			: this(filterField.ToEnumString(), filterValue)
		{
		}
	}
}
