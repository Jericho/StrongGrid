using System;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be like a value.
	/// </summary>
	/// <remarks>
	/// The percentage symbol, %, is the wildcard character.
	/// </remarks>
	/// <typeparam name="TEnum">The type containing an enum of fields that can used for searching/segmenting.</typeparam>
	public class SearchCriteriaLike<TEnum> : SearchCriteria<TEnum>
		where TEnum : Enum
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaLike{TEnum}"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaLike(TEnum filterField, object filterValue)
			: base(filterField, SearchComparisonOperator.Like, filterValue)
		{
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid segmenting API.
		/// </summary>
		/// <returns>The string representation of the operator.</returns>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()} ";
		}
	}
}
