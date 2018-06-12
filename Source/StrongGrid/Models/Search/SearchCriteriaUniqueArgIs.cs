using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be a value
	/// </summary>
	public class SearchCriteriaUniqueArgIs : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgIs"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg</param>
		/// <param name="filterValues">The filter values</param>
		public SearchCriteriaUniqueArgIs(string uniqueArgName, IEnumerable<object> filterValues)
			: base(uniqueArgName, SearchConditionOperator.Is, filterValues)
		{
		}
	}
}
