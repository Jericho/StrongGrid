using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	public class SearchCriteriaNotIn : SearchCriteria
	{
		public SearchCriteriaNotIn()
		{
		}

		public SearchCriteriaNotIn(FilterField filterField, IEnumerable<object> values)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.NotIn;
			base.FilterValue = values;
		}
	}
}
