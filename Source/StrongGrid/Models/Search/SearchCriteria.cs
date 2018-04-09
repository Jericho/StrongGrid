using StrongGrid.Utilities;
using System;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	public abstract class SearchCriteria
	{
		public FilterField FilterField { get; protected set; }

		public SearchConditionOperator FilterOperator { get; protected set; }

		public object FilterValue { get; protected set; }

		public SearchCriteria()
		{
		}

		public SearchCriteria(FilterField filterField, SearchConditionOperator filterOperator, object filterValue)
		{
			this.FilterField = filterField;
			this.FilterOperator = filterOperator;
			this.FilterValue = filterValue;
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

		public static string ValueAsString(object value)
		{
			var valueAsString = value is DateTime dateValue ? $"TIMESTAMP \"{dateValue.ToUniversalTime():u}\"" : $"\"{value}\"";
			return valueAsString;
		}
	}
}
