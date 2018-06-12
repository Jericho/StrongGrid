namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be greater than a value.
	/// </summary>
	public class SearchCriteriaUniqueArgGreaterThan : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgGreaterThan"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaUniqueArgGreaterThan(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchConditionOperator.GreaterThan, filterValue)
		{
		}
	}
}
