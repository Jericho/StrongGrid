namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be greater than or equal to a value.
	/// </summary>
	public class SearchCriteriaUniqueArgGreaterEqual : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgGreaterEqual"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaUniqueArgGreaterEqual(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchConditionOperator.GreaterEqual, filterValue)
		{
		}
	}
}
