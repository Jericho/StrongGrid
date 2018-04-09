namespace StrongGrid.Models.Search
{
	public class SearchCriteriaGreaterEqual : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaGreaterEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaGreaterEqual(FilterField filterField, object filterValue)
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.GreaterEqual;
			FilterValue = filterValue;
		}
	}
}
