using StrongGrid.Utilities;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be NULL
	/// </summary>
	public class SearchCriteriaUniqueArgNotNull : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgNotNull"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg</param>
		public SearchCriteriaUniqueArgNotNull(string uniqueArgName)
			: base(uniqueArgName, SearchConditionOperator.NotNull, null)
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
