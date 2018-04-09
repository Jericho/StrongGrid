namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be like a value
	/// </summary>
	public class SearchCriteriaLike : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaLike"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaLike(FilterField filterField, object filterValue)
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.Like;
			FilterValue = filterValue;
		}
	}
}
