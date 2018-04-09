namespace StrongGrid.Models.Search
{
	public class SearchCriteriaGreaterThan : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaGreaterThan"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaGreaterThan(FilterField filterField, object filterValue)
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.GreaterThan;
			FilterValue = filterValue;
		}
	}
}
