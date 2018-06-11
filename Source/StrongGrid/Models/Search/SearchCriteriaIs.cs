using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be a value
	/// </summary>
	public class SearchCriteriaIs : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIs"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterValues">The filter values</param>
		public SearchCriteriaIs(FilterField filterField, IEnumerable<object> filterValues)
			: base(filterField, SearchConditionOperator.Is, filterValues)
		{
		}
	}
}
