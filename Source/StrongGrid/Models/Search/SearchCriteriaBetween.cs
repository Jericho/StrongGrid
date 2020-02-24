using System;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be between a lower value and an upper value.
	/// </summary>
	/// <typeparam name="TEnum">The type containing an enum of fields that can used for searching/segmenting.</typeparam>
	public class SearchCriteriaBetween<TEnum> : SearchCriteria<TEnum>
		where TEnum : Enum
	{
		/// <summary>
		/// Gets the upper value.
		/// </summary>
		public object UpperValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaBetween{TEnum}"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="lowerValue">The lower value.</param>
		/// <param name="upperValue">The upper value.</param>
		public SearchCriteriaBetween(TEnum filterField, object lowerValue, object upperValue)
			: base(filterField, SearchConditionOperator.Between, lowerValue)
		{
			UpperValue = upperValue;
		}

		/// <summary>
		/// Converts the filter value into a string as expected by the SendGrid Email Activities API.
		/// </summary>
		/// <returns>The string representation of the value.</returns>
		public override string ConvertValueToString()
		{
			return $"{ConvertToString(FilterValue)} AND {ConvertToString(UpperValue)}";
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
