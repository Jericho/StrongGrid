using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	public class SearchCriteriaIn : SearchCriteria
	{
		public SearchCriteriaIn()
		{
		}

		public SearchCriteriaIn(FilterField filterField, IEnumerable<object> values)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.In;
			base.FilterValue = values;
		}
	}
}
