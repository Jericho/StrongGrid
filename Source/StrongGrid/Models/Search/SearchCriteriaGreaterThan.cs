namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be greater than a value
	/// </summary>
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
