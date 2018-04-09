using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	public class SearchCriteriaIs : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIs"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValues">The filter values</param>
		public SearchCriteriaIs(FilterField filterField, IEnumerable<object> filterValues)
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.Is;
			FilterValue = filterValues;
		}
	}
}
