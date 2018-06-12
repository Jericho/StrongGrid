namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be less than a lower value or greater than an upper value
	/// </summary>
	public class SearchCriteriaUniqueArgNotBetween : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Gets the upper value
		/// </summary>
		public object UpperValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgNotBetween"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg</param>
		/// <param name="lowerValue">The lower value</param>
		/// <param name="upperValue">The upper value</param>
		public SearchCriteriaUniqueArgNotBetween(string uniqueArgName, object lowerValue, object upperValue)
			: base(uniqueArgName, SearchConditionOperator.NotBetween, lowerValue)
		{
			UpperValue = upperValue;
		}

		/// <summary>
		/// Returns a string representation of the search criteria
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria</returns>
		public override string ToString()
		{
			return $"(unique_args['{UniqueArgName}'] NOT BETWEEN {SearchCriteria.ValueAsString(FilterValue)} AND {SearchCriteria.ValueAsString(UpperValue)})";
		}
	}
}
