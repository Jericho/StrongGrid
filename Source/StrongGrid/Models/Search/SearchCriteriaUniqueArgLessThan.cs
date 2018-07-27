namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be less than a value.
	/// </summary>
	public class SearchCriteriaUniqueArgLessThan : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgLessThan"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaUniqueArgLessThan(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchConditionOperator.LessThan, filterValue)
		{
		}
	}
}
