using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Base class for search criteria classes.
	/// </summary>
	public abstract class SearchCriteria : ISearchCriteria
	{
		/// <summary>
		/// Gets or sets the filter used to filter the result.
		/// </summary>
		public string FilterField { get; protected set; }

		/// <summary>
		/// Gets or sets the operator used to filter the result.
		/// </summary>
		public SearchComparisonOperator FilterOperator { get; protected set; }

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
		public SearchCriteria(string filterField, SearchComparisonOperator filterOperator, object filterValue)
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
		/// Returns the string representation of a given value as expected by the SendGrid segmenting API.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The <see cref="string"/> representation of the value.</returns>
		public static string ConvertToString(object value)
		{
			if (value is DateTime dateValue)
			{
				return $"\"{dateValue.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}\"";
			}
			else if (value is string stringValue)
			{
				return $"\"{stringValue ?? string.Empty}\"";
			}
			else if (value is Enum enumValue)
			{
				return $"\"{enumValue.ToEnumString()}\"";
			}
			else if (value is IEnumerable values)
			{
				return $"({string.Join(",", values.Cast<object>().Select(e => ConvertToString(e)))})";
			}
			else if (value.IsNumber())
			{
				return value.ToString();
			}
			else if (value is bool boolValue)
			{
				return boolValue ? "true" : "false";
			}
			else if (value is TimeSpan timespanValue)
			{
				if (timespanValue.TotalMinutes == 0) return $"{timespanValue.TotalSeconds} second";
				else if (timespanValue.TotalHours == 0) return $"{timespanValue.TotalMinutes} minute";
				else if (timespanValue.TotalDays == 0) return $"{timespanValue.TotalHours} hour";
				else return $"{timespanValue.TotalDays} day";

				// .NET Timespan does not support 'TotalMonth' and 'TotalYear'
				/*
					else if (timespanValue.TotalMonth == 0) return $"{timespanValue.TotalDays} day";
					else if (timespanValue.TotalYear == 0) return $"{timespanValue.TotalMonth} month";
					else return $"{timespanValue.TotalYear} year";
				*/
			}

			return $"\"{value}\"";
		}

		/// <summary>
		/// Converts the filter value into a string as expected by the SendGrid segmenting API.
		/// Can be overridden in subclasses if the value needs special formatting.
		/// </summary>
		/// <returns>The string representation of the value.</returns>
		public virtual string ConvertValueToString()
		{
			return ConvertToString(FilterValue);
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid segmenting API.
		/// Can be overridden in subclasses if the operator needs special formatting.
		/// </summary>
		/// <returns>The string representation of the operator.</returns>
		public virtual string ConvertOperatorToString()
		{
			return FilterOperator.ToEnumString();
		}

		/// <summary>
		/// Returns a string representation of the search criteria.
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria.</returns>
		public override string ToString()
		{
			var filterOperator = ConvertOperatorToString();
			var filterValue = ConvertValueToString();
			return $"{FilterField}{filterOperator}{filterValue}";
		}
	}

	/// <summary>
	/// Base class for search criteria classes.
	/// </summary>
	/// <typeparam name="TEnum">The type containing an enum of fields that can used for searching/segmenting.</typeparam>
	public abstract class SearchCriteria<TEnum> : SearchCriteria
		where TEnum : Enum
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteria{TEnum}"/> class.
		/// </summary>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterOperator">The filter operator.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteria(TEnum filterField, SearchComparisonOperator filterOperator, object filterValue)
			: base(filterField.ToEnumString(), filterOperator, filterValue)
		{
		}
	}
}
