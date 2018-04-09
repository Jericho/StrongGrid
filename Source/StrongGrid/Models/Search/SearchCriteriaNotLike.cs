namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to not be like a value
	/// </summary>
	public class SearchCriteriaNotLike : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotLike"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaNotLike(FilterField filterField, object filterValue)
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.NotLike;
			FilterValue = filterValue;
		}
	}
}
