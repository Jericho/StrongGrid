namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be like a value.
	/// </summary>
	public class SearchCriteriaUniqueArgLike : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgLike"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaUniqueArgLike(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchComparisonOperator.Like, filterValue)
		{
		}

		/// <inheritdoc/>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()} ";
		}
	}
}
