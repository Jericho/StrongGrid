using System;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for a field to contain a value.
	/// </summary>
	/// <typeparam name="TEnum">The type containing an enum of fields that can used for searching/segmenting.</typeparam>
	public class SearchCriteriaContains<TEnum> : SearchCriteria<TEnum>
		where TEnum : Enum
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaContains{TEnum}"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaContains(TEnum filterField, object filterValue)
			: base(filterField, SearchComparisonOperator.Contains, filterValue)
		{
		}

		/// <summary>
		/// Returns a string representation of the search criteria.
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria.</returns>
		public override string ToString()
		{
			var filterOperator = ConvertOperatorToString();
			var filterValue = ConvertValueToString();
			return $"{filterOperator}({FilterField},{filterValue})";
		}
	}
}
