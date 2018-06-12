namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to not be a value
	/// </summary>
	public class SearchCriteriaUniqueArgIsNot : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgIsNot"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaUniqueArgIsNot(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchConditionOperator.IsNot, filterValue)
		{
		}
	}
}
