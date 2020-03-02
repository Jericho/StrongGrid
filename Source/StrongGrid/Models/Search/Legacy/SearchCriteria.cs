using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Search.Legacy
{
	/// <summary>
	/// Base class for search criteria classes.
	/// </summary>
	public abstract class SearchCriteria : ISearchCriteria
	{
		/// <summary>
		/// Gets or sets the filter used to filter the result.
		/// </summary>
		public EmailActivitiesFilterField FilterField { get; protected set; }

		/// <summary>
		/// Gets or sets the operator used to filter the result.
		/// </summary>
		public SearchConditionOperator FilterOperator { get; protected set; }

		/// <summary>
		/// Gets or sets the value used to filter the result.
		/// </summary>
		public object FilterValue { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteria"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterOperator">The filter operator.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteria(EmailActivitiesFilterField filterField, SearchConditionOperator filterOperator, object filterValue)
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
		/// <param name="value">The value.</param>
		/// <returns>The <see cref="string"/> representation of the value.</returns>
		public static string ConvertToString(object value)
		{
			if (value is DateTime dateValue)
			{
				return $"TIMESTAMP \"{dateValue.ToUniversalTime():u}\"";
			}
			else if (value is string stringValue)
			{
				return $"\"{stringValue ?? string.Empty}\"";
			}
			else if (value is Enum enumValue)
			{
				return $"\"{enumValue.GetAttributeOfType<EnumMemberAttribute>()?.Value ?? value.ToString()}\"";
			}
			else if (value is IEnumerable values)
			{
				return $"({string.Join(",", values.Cast<object>().Select(e => ConvertToString(e)))})";
			}
			else if (value.IsNumber())
			{
				return value.ToString();
			}

			return $"\"{value}\"";
		}

		/// <summary>
		/// Converts the filter value into a string as expected by the SendGrid Email Activities API.
		/// Can be overridden in subclasses if the value needs special formatting.
		/// </summary>
		/// <returns>The string representation of the value.</returns>
		public virtual string ConvertValueToString()
		{
			return ConvertToString(FilterValue);
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid Email Activities API.
		/// Can be overridden in subclasses if the operator needs special formatting.
		/// </summary>
		/// <returns>The string representation of the operator.</returns>
		public virtual string ConvertOperatorToString()
		{
			return FilterOperator.GetAttributeOfType<EnumMemberAttribute>()?.Value ?? FilterOperator.ToString();
		}

		/// <summary>
		/// Returns a string representation of the search criteria.
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria.</returns>
		public override string ToString()
		{
			var fieldName = FilterField.GetAttributeOfType<EnumMemberAttribute>().Value;
			var filterOperator = ConvertOperatorToString();
			var filterValue = ConvertValueToString();
			return $"{fieldName}{filterOperator}{filterValue}";
		}
	}
}
