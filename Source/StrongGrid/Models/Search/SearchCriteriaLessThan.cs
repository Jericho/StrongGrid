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
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaLessThan(FilterField filterField, object filterValue)
			: base(filterField, SearchConditionOperator.LessThan, filterValue)
		{
		}
	}
}
