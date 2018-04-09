using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	public class SearchCriteriaNotLike : SearchCriteria
	{
		public SearchCriteriaNotLike()
		{
		}

		public SearchCriteriaNotLike(FilterField filterField, IEnumerable<object> values)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.NotLike;
			base.FilterValue = values;
		}
	}
}
