using System;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be not NULL.
	/// </summary>
	/// <typeparam name="TEnum">The type containing an enum of fields that can used for searching/segmenting.</typeparam>
	public class SearchCriteriaIsNotNull<TEnum> : SearchCriteria<TEnum>
		where TEnum : Enum
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNotNull{TEnum}"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNotNull(TEnum filterField)
			: base(filterField, SearchComparisonOperator.IsNotNull, null)
		{
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid Email Activities API.
		/// </summary>
		/// <returns>The string representation of the operator.</returns>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()}";
		}
	}
}
