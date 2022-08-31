namespace StrongGrid.Models.Search
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
			: base(uniqueArgName, SearchComparisonOperator.NotBetween, lowerValue)
		{
			UpperValue = upperValue;
		}

		/// <inheritdoc/>
		public override string ConvertValueToString(char quote)
		{
			return $"{SearchCriteria.ConvertToString(FilterValue, quote)} AND {SearchCriteria.ConvertToString(UpperValue, quote)}";
		}

		/// <inheritdoc/>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()} ";
		}
	}
}
