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

		/// <summary>
		/// Converts the filter value into a string as expected by the SendGrid segmenting API.
		/// </summary>
		/// <returns>The string representation of the value.</returns>
		public override string ConvertValueToString()
		{
			return $"{ConvertToString(FilterValue)} AND {ConvertToString(UpperValue)}";
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid segmenting API.
		/// </summary>
		/// <returns>The string representation of the operator.</returns>
		public override string ConvertOperatorToString()
		{
			return $" {base.ConvertOperatorToString()} ";
		}
	}
}
