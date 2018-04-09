namespace StrongGrid.Models.Search
{
	public class SearchCriteriaLessEqual : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaLessEqual"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaLessEqual(FilterField filterField, object filterValue)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.LessEqual;
			base.FilterValue = filterValue;
		}
	}
}
