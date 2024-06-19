using System;
using System.Collections;
using System.Linq;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Base class for search criteria classes.
	/// </summary>
	public abstract class SearchCriteria : ISearchCriteria
	{
		private const char QuoteV1 = '"';
		private const char QuoteV2 = '\'';

		/// <summary>
		/// Gets or sets the name of the table.
		/// </summary>
		public FilterTable FilterTable { get; protected set; }

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
		/// <param name="filterTable">The filter table.</param>
		/// <param name="filterField">The filter field.</param>
		/// <param name="filterOperator">The filter operator.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteria(FilterTable filterTable, string filterField, SearchComparisonOperator filterOperator, object filterValue)
		{
			this.FilterTable = filterTable;
			this.FilterField = filterField;
			this.FilterOperator = filterOperator;
			this.FilterValue = filterValue;
		}

		/// <summary>
		/// Returns the string representation of a given value as expected by the SendGrid segmenting API.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="queryLanguageVersion">The desired query version.</param>
		/// <returns>The <see cref="string"/> representation of the value.</returns>
		public static string ConvertToString(object value, QueryLanguageVersion queryLanguageVersion)
		{
			var quote = queryLanguageVersion switch
			{
				QueryLanguageVersion.Unspecified => QuoteV1,
				QueryLanguageVersion.Version1 => QuoteV1,
				QueryLanguageVersion.Version2 => QuoteV2,
				_ => throw new ArgumentOutOfRangeException(nameof(queryLanguageVersion), queryLanguageVersion, null)
			};

			if (value is DateTime dateValue)
			{
				if (queryLanguageVersion == QueryLanguageVersion.Version1)
				{
					return $"TIMESTAMP {quote}{dateValue.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}{quote}";
				}
				else
				{
					return $"{quote}{dateValue.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}{quote}";
				}
			}
			else if (value is string stringValue)
			{
				return $"{quote}{stringValue ?? string.Empty}{quote}";
			}
			else if (value is Enum enumValue)
			{
				return $"{quote}{enumValue.ToEnumString()}{quote}";
			}
			else if (value is IEnumerable values)
			{
				return $"[{string.Join(",", values.Cast<object>().Select(e => ConvertToString(e, queryLanguageVersion)))}]";
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
		/// <param name="queryLanguageVersion">The desired query version.</param>
		/// <returns>The string representation of the value.</returns>
		public virtual string ConvertValueToString(QueryLanguageVersion queryLanguageVersion)
		{
			return ConvertToString(FilterValue, queryLanguageVersion);
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

		/// <inheritdoc/>
		public override string ToString()
		{
			var filterOperator = ConvertOperatorToString();
			var filterValue = ConvertValueToString(QueryLanguageVersion.Version1);

			return $"{FilterField}{filterOperator}{filterValue}";
		}

		/// <inheritdoc/>
		public virtual string ToString(string tableAlias)
		{
			var filterOperator = ConvertOperatorToString();
			var filterValue = ConvertValueToString(QueryLanguageVersion.Version2);
			var filterField = string.IsNullOrEmpty(tableAlias) ? FilterField : $"{tableAlias}.{FilterField}";

			return $"{filterField}{filterOperator}{filterValue}";
		}
	}
}
