using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be absent from an enumeration of values.
	/// </summary>
	public class SearchCriteriaNotIn : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotIn"/> class.
		/// </summary>
		/// <param name="filterTable">The filter table.</param>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValues">The filter values.</param>
		public SearchCriteriaNotIn(FilterTable filterTable, string filterField, IEnumerable<object> filterValues)
			: base(filterTable, filterField, SearchComparisonOperator.NotIn, filterValues)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotIn"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValues">The filter values.</param>
		public SearchCriteriaNotIn(ContactsFilterField filterField, IEnumerable<object> filterValues)
			: this(FilterTable.Contacts, filterField.ToEnumString(), filterValues)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotIn"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValues">The filter values.</param>
		public SearchCriteriaNotIn(EmailActivitiesFilterField filterField, IEnumerable<object> filterValues)
			: this(FilterTable.EmailActivities, filterField.ToEnumString(), filterValues)
		{
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
