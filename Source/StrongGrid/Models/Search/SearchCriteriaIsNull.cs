namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be NULL.
	/// </summary>
	public class SearchCriteriaIsNull : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNull"/> class.
		/// </summary>
		/// <param name="filterTable">The filter table.</param>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNull(FilterTable filterTable, string filterField)
			: base(filterTable, filterField, SearchComparisonOperator.IsNull, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNull(ContactsFilterField filterField)
			: this(FilterTable.Contacts, filterField.ToEnumString())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIsNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		public SearchCriteriaIsNull(EmailActivitiesFilterField filterField)
			: this(FilterTable.EmailActivities, filterField.ToEnumString())
		{
		}

		/// <inheritdoc/>
		public override string ConvertValueToString(QueryLanguageVersion queryLanguageVersion)
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
