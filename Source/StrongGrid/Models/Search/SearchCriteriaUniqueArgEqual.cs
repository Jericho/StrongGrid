namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be equal to a value.
	/// </summary>
	public class SearchCriteriaUniqueArgEqual : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgEqual"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaUniqueArgEqual(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchComparisonOperator.Equal, filterValue)
		{
		}
	}
}
