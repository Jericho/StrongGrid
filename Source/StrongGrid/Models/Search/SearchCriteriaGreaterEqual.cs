namespace StrongGrid.Models.Search
{
	public class SearchCriteriaGreaterEqual : SearchCriteria
	{
		public SearchCriteriaGreaterEqual()
		{
		}

		public SearchCriteriaGreaterEqual(FilterField filterField, object filterValue)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.GreaterEqual;
			base.FilterValue = filterValue;
		}
	}
}
