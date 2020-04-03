using System;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be NULL.
	/// </summary>
	/// <typeparam name="TEnum">The type containing an enum of fields that can used for searching/segmenting.</typeparam>
	public class SearchCriteriaIsNull<TEnum> : SearchCriteria<TEnum>
		where TEnum : Enum
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNull{TEnum}"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNull(TEnum filterField)
			: base(filterField, SearchComparisonOperator.IsNull, null)
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
