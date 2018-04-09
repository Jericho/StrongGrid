namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to not be a value
	/// </summary>
	public class SearchCriteriaIsNot : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNot"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaIsNot(FilterField filterField, object filterValue)
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.IsNot;
			FilterValue = filterValue;
		}
	}
}
