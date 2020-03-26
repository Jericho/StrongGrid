using System;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to not be like a value.
	/// </summary>
	/// <typeparam name="TEnum">The type containing an enum of fields that can used for searching/segmenting.</typeparam>
	public class SearchCriteriaNotLike<TEnum> : SearchCriteria<TEnum>
		where TEnum : Enum
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotLike{TEnum}"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaNotLike(TEnum filterField, object filterValue)
			: base(filterField, SearchComparisonOperator.NotLike, filterValue)
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
