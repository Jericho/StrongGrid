namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be less than or equal to a value.
	/// </summary>
	public class SearchCriteriaUniqueArgLessEqual : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgLessEqual"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaUniqueArgLessEqual(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchConditionOperator.LessEqual, filterValue)
		{
		}
	}
}
