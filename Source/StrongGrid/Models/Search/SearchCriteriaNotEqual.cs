namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be different than a value
	/// </summary>
	public class SearchCriteriaNotEqual : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaNotEqual(FilterField filterField, object filterValue)
			: base(filterField, SearchConditionOperator.NotEqual, filterValue)
		{
		}
	}
}
