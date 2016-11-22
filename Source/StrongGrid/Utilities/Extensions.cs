using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace StrongGrid.Utilities
{
	public static class Extensions
	{
		private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime FromUnixTime(this long unixTime)
		{
			return EPOCH.AddSeconds(unixTime);
		}

		public static long ToUnixTime(this DateTime date)
		{
			return Convert.ToInt64((date.ToUniversalTime() - EPOCH).TotalSeconds);
		}

		public static string GetDescription(this Enum value)
		{
			var fieldInfo = value.GetType().GetField(value.ToString());
			if (fieldInfo == null) return value.ToString();

			var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).ToArray();
			if (attributes == null || attributes.Length == 0) return value.ToString();

			var descriptionAttribute = attributes[0] as DescriptionAttribute;
			return (descriptionAttribute == null ? value.ToString() : descriptionAttribute.Description);
		}

		public static T ConverDescriptiontToEnum<T>(this string description)
		{
			var fields = typeof(T).GetFields();
			foreach (var fieldInfo in fields)
			{
				var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>();
				if (attributes.Any(a => a.Description == description))
				{
					return (T)Enum.Parse(typeof(T), fieldInfo.Name, true);
				}
			}

			var message = string.Format("'{0}' is not a valid enumeration of '{1}'", description, typeof(T).Name);
			throw new Exception(message);
		}

		public static void EnsureSuccess(this HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode) return;

			var content = string.Empty;
			if (response.Content != null)
			{
				content = response.Content.ReadAsStringAsync().Result;
				response.Content.Dispose();

				try
				{
					// Response looks like this:
					// {
					//   "errors": [
					//     {
					//       "message": "An error has occured",
					//       "field": null,
					//       "help": null
					//     }
					//  ]
					// }
					// We use a dynamic object to get rid of the 'errors' property and get the first error message
					dynamic dynamicObject = JObject.Parse(content);
					dynamic dynamicArray = dynamicObject.errors;
					dynamic firstError = dynamicArray.First;

					content = firstError.message.ToString();
				} catch
				{
					// Intentionally ignore parsing errors
				}
			}

			if (string.IsNullOrEmpty(content))
			{
				content = string.Format("StatusCode: {0}", response.StatusCode);
			}

			throw new Exception(content);
		}
	}
}
