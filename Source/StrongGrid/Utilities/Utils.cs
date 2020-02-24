using Microsoft.IO;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Utils.
	/// </summary>
	internal static class Utils
	{
		public static DateTime Epoch { get; } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static RecyclableMemoryStreamManager MemoryStreamManager { get; } = new RecyclableMemoryStreamManager();

		/// <summary>
		/// Returns the string representation of a given value as expected by the SendGrid Email Activities API.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The <see cref="string"/> representation of the value.</returns>
		public static string ConvertValueToStringForSearching(object value)
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
				return $"({string.Join(",", values.Cast<object>().Select(e => ConvertValueToStringForSearching(e)))})";
			}
			else if (value.IsNumber())
			{
				return value.ToString();
			}

			return $"\"{value}\"";
		}
	}
}
