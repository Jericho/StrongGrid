using System.Collections.Generic;

namespace StrongGrid.Models.Search.Legacy
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be present in an enumeration of values.
	/// </summary>
	public class SearchCriteriaUniqueArgIn : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgIn"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="filterValues">The filter values.</param>
		public SearchCriteriaUniqueArgIn(string uniqueArgName, IEnumerable<object> filterValues)
			: base(uniqueArgName, SearchConditionOperator.In, filterValues)
		{
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid Email Activities API.
		/// </summary>
		/// <returns>The string representation of the operator.</returns>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()} ";
		}
	}
}
