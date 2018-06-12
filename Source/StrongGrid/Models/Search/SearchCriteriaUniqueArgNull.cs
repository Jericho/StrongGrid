using StrongGrid.Utilities;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be NULL
	/// </summary>
	public class SearchCriteriaUniqueArgNull : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgNull"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg</param>
		public SearchCriteriaUniqueArgNull(string uniqueArgName)
			: base(uniqueArgName, SearchConditionOperator.Null, null)
		{
		}

		/// <summary>
		/// Returns a string representation of the search criteria
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria</returns>
		public override string ToString()
		{
			var filterOperator = FilterOperator.GetAttributeOfType<EnumMemberAttribute>().Value;
			return $"(unique_args['{UniqueArgName}'] {filterOperator})";
		}
	}
}
