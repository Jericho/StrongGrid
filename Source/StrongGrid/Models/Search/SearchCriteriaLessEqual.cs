namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be less than or equal to a value.
	/// </summary>
	public class SearchCriteriaLessEqual : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaLessEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaLessEqual(string filterField, object filterValue)
			: base(filterField, SearchComparisonOperator.LessEqual, filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaLessEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaLessEqual(ContactsFilterField filterField, object filterValue)
			: this(filterField.ToEnumString(), filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaLessEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaLessEqual(EmailActivitiesFilterField filterField, object filterValue)
			: this(filterField.ToEnumString(), filterValue)
		{
		}
	}
}
