namespace StrongGrid.Models.Search.Legacy
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be less than a lower value or greater than an upper value.
	/// </summary>
	public class SearchCriteriaUniqueArgNotBetween : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Gets the upper value.
		/// </summary>
		public object UpperValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgNotBetween"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="lowerValue">The lower value.</param>
		/// <param name="upperValue">The upper value.</param>
		public SearchCriteriaUniqueArgNotBetween(string uniqueArgName, object lowerValue, object upperValue)
			: base(uniqueArgName, SearchConditionOperator.NotBetween, lowerValue)
		{
			UpperValue = upperValue;
		}

		/// <summary>
		/// Converts the filter value into a string as expected by the SendGrid Email Activities API.
		/// </summary>
		/// <returns>The string representation of the value.</returns>
		public override string ConvertValueToString()
		{
			return $"{SearchCriteria.ConvertToString(FilterValue)} AND {SearchCriteria.ConvertToString(UpperValue)}";
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
