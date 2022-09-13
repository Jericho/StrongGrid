namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be NULL.
	/// </summary>
	public class SearchCriteriaUniqueArgNull : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgNull"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		public SearchCriteriaUniqueArgNull(string uniqueArgName)
			: base(uniqueArgName, SearchComparisonOperator.IsNull, null)
		{
		}

		/// <inheritdoc/>
		public override string ConvertValueToString(char quote)
		{
			return string.Empty;
		}

		/// <inheritdoc/>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()}";
		}
	}
}
