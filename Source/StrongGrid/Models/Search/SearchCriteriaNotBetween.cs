namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be less than a lower value or greater than an upper value.
	/// </summary>
	public class SearchCriteriaNotBetween : SearchCriteria
	{
		/// <summary>
		/// Gets the upper value.
		/// </summary>
		public object UpperValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotBetween"/> class.
		/// </summary>
		/// <param name="filterTable">The filter table.</param>
		/// <param name="filterField">The filter field.</param>
		/// <param name="lowerValue">The lower value.</param>
		/// <param name="upperValue">The upper value.</param>
		public SearchCriteriaNotBetween(FilterTable filterTable, string filterField, object lowerValue, object upperValue)
			: base(filterTable, filterField, SearchComparisonOperator.NotBetween, lowerValue)
		{
			UpperValue = upperValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotBetween"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="lowerValue">The lower value.</param>
		/// <param name="upperValue">The upper value.</param>
		public SearchCriteriaNotBetween(ContactsFilterField filterField, object lowerValue, object upperValue)
			: this(FilterTable.Contacts, filterField.ToEnumString(), lowerValue, upperValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotBetween"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="lowerValue">The lower value.</param>
		/// <param name="upperValue">The upper value.</param>
		public SearchCriteriaNotBetween(EmailActivitiesFilterField filterField, object lowerValue, object upperValue)
			: this(FilterTable.EmailActivities, filterField.ToEnumString(), lowerValue, upperValue)
		{
		}

		/// <inheritdoc/>
		public override string ConvertValueToString(QueryLanguageVersion queryLanguageVersion)
		{
			return $"{ConvertToString(FilterValue, queryLanguageVersion)} AND {ConvertToString(UpperValue, queryLanguageVersion)}";
		}

		/// <inheritdoc/>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()} ";
		}
	}
}
