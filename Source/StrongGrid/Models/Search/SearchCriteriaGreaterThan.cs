namespace StrongGrid.Models.Search
{
	public class SearchCriteriaGreaterThan : SearchCriteria
	{
		public SearchCriteriaGreaterThan()
		{
		}

		public SearchCriteriaGreaterThan(FilterField filterField, object filterValue)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.GreaterThan;
			base.FilterValue = filterValue;
		}
	}
}
