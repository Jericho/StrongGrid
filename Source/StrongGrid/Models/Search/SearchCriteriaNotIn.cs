using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	public class SearchCriteriaNotIn : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotIn"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValues">The filter values</param>
		public SearchCriteriaNotIn(FilterField filterField, IEnumerable<object> filterValues)
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.NotIn;
			FilterValue = filterValues;
		}
	}
}
