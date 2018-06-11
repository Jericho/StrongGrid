namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be greater than or equal to a value
	/// </summary>
	public class SearchCriteriaGreaterEqual : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaGreaterEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaGreaterEqual(FilterField filterField, object filterValue)
			: base(filterField, SearchConditionOperator.GreaterEqual, filterValue)
		{
		}
	}
}
