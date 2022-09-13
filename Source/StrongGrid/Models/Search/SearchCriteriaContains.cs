namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for a field to contain a value.
	/// </summary>
	public class SearchCriteriaContains : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaContains"/> class.
		/// </summary>
		/// <param name="filterTable">The filter table.</param>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaContains(FilterTable filterTable, string filterField, object filterValue)
			: base(filterTable, filterField, SearchComparisonOperator.Contains, filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaContains"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaContains(ContactsFilterField filterField, object filterValue)
			: this(FilterTable.Contacts, filterField.ToEnumString(), filterValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaContains"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaContains(EmailActivitiesFilterField filterField, object filterValue)
			: this(FilterTable.EmailActivities, filterField.ToEnumString(), filterValue)
		{
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			var filterOperator = "CONTAINS"; // This is the operator from SendGrid's query DSL version 1. "ARRAY_CONTAINS" is the operator from version 2.
			var filterValue = ConvertValueToString('"');

			return $"{filterOperator}({FilterField},{filterValue})";
		}

		/// <inheritdoc/>
		public override string ToString(string tableAlias)
		{
			var filterOperator = ConvertOperatorToString();
			var filterValue = ConvertValueToString('\'');
			var filterField = string.IsNullOrEmpty(tableAlias) ? FilterField : $"{tableAlias}.{FilterField}";

			return $"{filterOperator}({filterField},{filterValue})";
		}
	}
}
