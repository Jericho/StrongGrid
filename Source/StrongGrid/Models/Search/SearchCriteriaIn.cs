using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be present in an enumeration of values
	/// </summary>
	public class SearchCriteriaIn : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIn"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValues">The filter values</param>
		public SearchCriteriaIn(FilterField filterField, IEnumerable<object> filterValues)
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.In;
			FilterValue = filterValues;
		}
	}
}
