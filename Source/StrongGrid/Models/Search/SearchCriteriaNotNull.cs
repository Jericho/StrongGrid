using StrongGrid.Utilities;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be not NULL
	/// </summary>
	public class SearchCriteriaNotNull : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		public SearchCriteriaNotNull(FilterField filterField)
			: base(filterField, SearchConditionOperator.NotNull, null)
		{
		}

		/// <summary>
		/// Returns a string representation of the search criteria
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria</returns>
		public override string ToString()
		{
			var filterOperator = FilterOperator.GetAttributeOfType<EnumMemberAttribute>().Value;
			return $"{FilterField} {filterOperator}";
		}
	}
}
