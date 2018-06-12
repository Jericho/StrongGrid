namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be not NULL
	/// </summary>
	public class SearchCriteriaNotNull : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		public SearchCriteriaNotNull(FilterField filterField)
			: base(filterField, SearchConditionOperator.NotNull, null)
		{
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid Email Activities API.
		/// </summary>
		/// <returns>The string representation of the operator</returns>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()}";
		}
	}
}
