namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search on the value of a custom tracking argument to be like a value
	/// </summary>
	public class SearchCriteriaUniqueArgLike : SearchCriteriaUniqueArg
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArgLike"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteriaUniqueArgLike(string uniqueArgName, object filterValue)
			: base(uniqueArgName, SearchConditionOperator.Like, filterValue)
		{
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid Email Activities API.
		/// </summary>
		/// <returns>The string representation of the operator</returns>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()} ";
		}
	}
}
