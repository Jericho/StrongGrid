using StrongGrid.Utilities;
using System;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Base class for search criteria classes
	/// </summary>
	public abstract class SearchCriteria : ISearchCriteria
	{
		/// <summary>
		/// Gets or sets the filter used to filter the result
		/// </summary>
		public FilterField FilterField { get; protected set; }

		/// <summary>
		/// Gets or sets the operator used to filter the result
		/// </summary>
		public SearchConditionOperator FilterOperator { get; protected set; }

		/// <summary>
		/// Gets or sets the value used to filter the result
		/// </summary>
		public object FilterValue { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteria"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="filterOperator">The filter operator</param>
		/// <param name="filterValue">The filter value</param>
		public SearchCriteria(FilterField filterField, SearchConditionOperator filterOperator, object filterValue)
		{
			this.FilterField = filterField;
			this.FilterOperator = filterOperator;
			this.FilterValue = filterValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteria"/> class.
		/// </summary>
		protected SearchCriteria()
		{
		}

		/// <summary>
		/// Returns the string representation of a given value as expected by the SendGrid Email Activities API.
		/// </summary>
		/// <param name="value">The value</param>
		/// <returns>The <see cref="string"/> representation of the value</returns>
		public static string ValueAsString(object value)
		{
			var valueAsString = value is DateTime dateValue ? $"TIMESTAMP \"{dateValue.ToUniversalTime():u}\"" : $"\"{value}\"";
			return valueAsString;
		}

		/// <summary>
		/// Returns a string representation of the search criteria
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria</returns>
		public override string ToString()
		{
			var fieldName = FilterField.GetAttributeOfType<EnumMemberAttribute>().Value;
			var filterOperator = FilterOperator.GetAttributeOfType<EnumMemberAttribute>().Value;
			var filterValue = ValueAsString(FilterValue);
			return $"{fieldName}{filterOperator}{filterValue}";
		}
	}
}
