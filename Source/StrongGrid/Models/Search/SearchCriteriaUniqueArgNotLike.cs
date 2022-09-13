namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to not be like a value.
	/// </summary>
	public class SearchCriteriaUniqueArgNotLike : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgNotLike"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaUniqueArgNotLike(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchComparisonOperator.NotLike, filterValue)
		{
		}

		/// <inheritdoc/>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()} ";
		}
	}
}
