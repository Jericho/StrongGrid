using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be present in an enumeration of values
	/// </summary>
	public class SearchCriteriaUniqueArgIn : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgIn"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg</param>
		/// <param name="filterValues">The filter values</param>
		public SearchCriteriaUniqueArgIn(string uniqueArgName, IEnumerable<object> filterValues)
			: base(uniqueArgName, SearchConditionOperator.In, filterValues)
		{
		}
	}
}
