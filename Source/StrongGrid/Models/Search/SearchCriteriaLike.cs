using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	public class SearchCriteriaLike : SearchCriteria
	{
		public SearchCriteriaLike()
		{
		}

		public SearchCriteriaLike(FilterField filterField, IEnumerable<object> values)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.Like;
			base.FilterValue = values;
		}
	}
}
