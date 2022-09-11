using System.Collections.Generic;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be present in an enumeration of values.
	/// </summary>
	public class SearchCriteriaIn : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIn"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValues">The filter values.</param>
		public SearchCriteriaIn(string filterField, IEnumerable<object> filterValues)
			: base(filterField, SearchComparisonOperator.In, filterValues)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIn"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValues">The filter values.</param>
		public SearchCriteriaIn(ContactsFilterField filterField, IEnumerable<object> filterValues)
			: this(filterField.ToEnumString(), filterValues)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaIn"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterValues">The filter values.</param>
		public SearchCriteriaIn(EmailActivitiesFilterField filterField, IEnumerable<object> filterValues)
			: this(filterField.ToEnumString(), filterValues)
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
