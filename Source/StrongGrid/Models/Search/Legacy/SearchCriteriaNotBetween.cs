namespace StrongGrid.Models.Search.Legacy
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be less than a lower value or greater than an upper value.
	/// </summary>
	public class SearchCriteriaNotBetween : SearchCriteria
	{
		/// <summary>
		/// Gets the upper value.
		/// </summary>
		public object UpperValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotBetween"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="lowerValue">The lower value.</param>
		/// <param name="upperValue">The upper value.</param>
		public SearchCriteriaNotBetween(EmailActivitiesFilterField filterField, object lowerValue, object upperValue)
			: base(filterField, SearchConditionOperator.NotBetween, lowerValue)
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
