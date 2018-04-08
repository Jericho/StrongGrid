namespace StrongGrid.Models.Search
{
	public class SearchCriteriaLessEqual : SearchCriteria
	{
		public SearchCriteriaLessEqual()
		{
		}

		public SearchCriteriaLessEqual(FilterField filterField, object filterValue)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.LessEqual;
			base.FilterValue = filterValue;
		}
	}
}
