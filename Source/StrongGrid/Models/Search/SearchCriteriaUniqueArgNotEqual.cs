namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be different than a value
	/// </summary>
	public class SearchCriteriaUniqueArgNotEqual : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgNotEqual"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaUniqueArgNotEqual(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchConditionOperator.NotEqual, filterValue)
		{
		}
	}
}
