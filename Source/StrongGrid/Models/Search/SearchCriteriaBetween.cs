namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be between a lower value and an upper value.
	/// </summary>
	public class SearchCriteriaBetween : SearchCriteria
	{
		/// <summary>
		/// Gets the upper value.
		/// </summary>
		public object UpperValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaBetween"/> class.
		/// </summary>
		/// <param name="filterTable">The filter table.</param>
		/// <param name="filterField">The filter field.</param>
		/// <param name="lowerValue">The lower value.</param>
		/// <param name="upperValue">The upper value.</param>
		public SearchCriteriaBetween(FilterTable filterTable, string filterField, object lowerValue, object upperValue)
			: base(filterTable, filterField, SearchComparisonOperator.Between, lowerValue)
		{
			UpperValue = upperValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaBetween"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="lowerValue">The lower value.</param>
		/// <param name="upperValue">The upper value.</param>
		public SearchCriteriaBetween(ContactsFilterField filterField, object lowerValue, object upperValue)
			: this(FilterTable.Contacts, filterField.ToEnumString(), lowerValue, upperValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaBetween"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="lowerValue">The lower value.</param>
		/// <param name="upperValue">The upper value.</param>
		public SearchCriteriaBetween(EmailActivitiesFilterField filterField, object lowerValue, object upperValue)
			: this(FilterTable.EmailActivities, filterField.ToEnumString(), lowerValue, upperValue)
		{
		}

		/// <inheritdoc/>
		public override string ConvertValueToString(char quote)
		{
			return $"{ConvertToString(FilterValue, quote)} AND {ConvertToString(UpperValue, quote)}";
		}

		/// <inheritdoc/>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()} ";
		}
	}
}
