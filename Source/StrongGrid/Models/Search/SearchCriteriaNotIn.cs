using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be absent from an enumeration of values
	/// </summary>
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
