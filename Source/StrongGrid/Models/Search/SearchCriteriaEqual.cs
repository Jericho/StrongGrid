namespace StrongGrid.Models.Search
{
	public class SearchCriteriaEqual : SearchCriteria
	{
		public SearchCriteriaEqual()
		{
		}

		public SearchCriteriaEqual(FilterField filterField, object filterValue)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.Equal;
			base.FilterValue = filterValue;
		}
	}
}
