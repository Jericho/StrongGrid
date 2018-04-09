namespace StrongGrid.Models.Search
{
	public class SearchCriteriaNotEqual : SearchCriteria
	{
		public SearchCriteriaNotEqual()
		{
		}

		public SearchCriteriaNotEqual(FilterField filterField, object filterValue)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.NotEqual;
			base.FilterValue = filterValue;
		}
	}
}
