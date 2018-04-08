using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	public class SearchCriteriaIs : SearchCriteria
	{
		public SearchCriteriaIs()
		{
		}

		public SearchCriteriaIs(FilterField filterField, IEnumerable<object> values)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.Is;
			base.FilterValue = values;
		}
	}
}
