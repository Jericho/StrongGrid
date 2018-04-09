namespace StrongGrid.Models.Search
{
	public class SearchCriteriaEqual : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaEqual(FilterField filterField, object filterValue)
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.Equal;
			FilterValue = filterValue;
		}
	}
}
