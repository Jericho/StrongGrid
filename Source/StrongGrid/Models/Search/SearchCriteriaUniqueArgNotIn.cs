using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be absent from an enumeration of values
	/// </summary>
	public class SearchCriteriaUniqueArgNotIn : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgNotIn"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg</param>
		/// <param name="filterValues">The filter values</param>
		public SearchCriteriaUniqueArgNotIn(string uniqueArgName, IEnumerable<object> filterValues)
			: base(uniqueArgName, SearchConditionOperator.NotIn, filterValues)
		{
		}
	}
}
