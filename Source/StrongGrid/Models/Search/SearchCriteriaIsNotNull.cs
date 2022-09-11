namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be not NULL.
	/// </summary>
	public class SearchCriteriaIsNotNull : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNotNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNotNull(string filterField)
			: base(filterField, SearchComparisonOperator.IsNotNull, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNotNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNotNull(ContactsFilterField filterField)
			: this(filterField.ToEnumString())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNotNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNotNull(EmailActivitiesFilterField filterField)
			: this(filterField.ToEnumString())
		{
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid segmenting API.
		/// </summary>
		/// <returns>The string representation of the operator.</returns>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()}";
		}
	}
}
