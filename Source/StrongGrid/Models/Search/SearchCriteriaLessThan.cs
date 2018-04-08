namespace StrongGrid.Models.Search
{
	public class SearchCriteriaLessThan : SearchCriteria
	{
		public SearchCriteriaLessThan()
		{
		}

		public SearchCriteriaLessThan(FilterField filterField, object filterValue)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.LessThan;
			base.FilterValue = filterValue;
		}
	}
}
