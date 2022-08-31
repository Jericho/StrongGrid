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
		/// <param name="filterTable">The filter table.</param>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNotNull(FilterTable filterTable, string filterField)
			: base(filterTable, filterField, SearchComparisonOperator.IsNotNull, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNotNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNotNull(ContactsFilterField filterField)
			: this(FilterTable.Contacts, filterField.ToEnumString())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNotNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNotNull(EmailActivitiesFilterField filterField)
			: this(FilterTable.EmailActivities, filterField.ToEnumString())
		{
		}

		/// <inheritdoc/>
		public override string ConvertValueToString(char quote)
		{
			return string.Empty;
		}

		/// <inheritdoc/>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()}";
		}
	}
}
