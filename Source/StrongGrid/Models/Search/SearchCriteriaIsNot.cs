using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	public class SearchCriteriaIsNot : SearchCriteria
	{
		public SearchCriteriaIsNot()
		{
		}

		public SearchCriteriaIsNot(FilterField filterField, IEnumerable<object> values)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.IsNot;
			base.FilterValue = values;
		}
	}
}
