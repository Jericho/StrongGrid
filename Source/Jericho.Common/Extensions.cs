using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Jericho.Common
{
	internal enum UnixTimePrecision
	{
		Seconds = 0,
		Milliseconds = 1
	}

	internal static class Extensions
	{
		private static readonly DateTime EPOCH = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private static readonly int DEFAULT_DEGREE_OF_PARALLELISM = Environment.ProcessorCount > 1 ? Environment.ProcessorCount / 2 : 1;

		private static readonly Dictionary<Type, string> _typeAliases = new()
		{
			{ typeof(byte), "byte" },
			{ typeof(sbyte), "sbyte" },
			{ typeof(short), "short" },
			{ typeof(ushort), "ushort" },
			{ typeof(int), "int" },
			{ typeof(uint), "uint" },
			{ typeof(long), "long" },
			{ typeof(ulong), "ulong" },
			{ typeof(float), "float" },
			{ typeof(double), "double" },
			{ typeof(decimal), "decimal" },
			{ typeof(object), "object" },
			{ typeof(bool), "bool" },
			{ typeof(char), "char" },
			{ typeof(string), "string" },
			{ typeof(void), "void" },
			{ typeof(nint), "nint" },
			{ typeof(nuint), "nuint" }
		};

		internal static DateTime FromUnixTime(this long unixTime, UnixTimePrecision precision = UnixTimePrecision.Seconds)
		{
			if (precision == UnixTimePrecision.Seconds) return EPOCH.AddSeconds(unixTime);
			if (precision == UnixTimePrecision.Milliseconds) return EPOCH.AddMilliseconds(unixTime);
			throw new ArgumentException($"Unknown precision: {precision}");
		}

		internal static long ToUnixTime(this DateTime date, UnixTimePrecision precision = UnixTimePrecision.Seconds)
		{
			var diff = date.ToUniversalTime() - EPOCH;
			if (precision == UnixTimePrecision.Seconds) return Convert.ToInt64(diff.TotalSeconds);
			if (precision == UnixTimePrecision.Milliseconds) return Convert.ToInt64(diff.TotalMilliseconds);
			throw new ArgumentException($"Unknown precision: {precision}");
		}

		internal static async Task<string> ReadAsStringAsync(this HttpContent httpContent, Encoding fallbackEncoding, CancellationToken cancellationToken = default)
		{
			if (httpContent == null) return string.Empty;

#if NET5_0_OR_GREATER
			var contentStream = await httpContent.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
			var contentStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
#endif
			var encoding = httpContent.GetEncoding(fallbackEncoding);
			using var ms = new MemoryStream();
			const int DefaultBufferSize = 81920;
			await contentStream.CopyToAsync(ms, DefaultBufferSize, cancellationToken).ConfigureAwait(false);
			ms.Position = 0;
			using var sr = new StreamReader(ms, encoding);
#if NET7_0_OR_GREATER
			var content = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
#else
			var content = await sr.ReadToEndAsync().ConfigureAwait(false);
#endif
			if (contentStream.CanSeek) contentStream.Position = 0;
			return content;
		}

		internal static Encoding GetEncoding(this HttpContent content, Encoding fallbackEncoding)
		{
			Encoding encoding = null;
			try
			{
				var charset = content?.Headers?.ContentType?.CharSet;
				if (!string.IsNullOrEmpty(charset)) encoding = Encoding.GetEncoding(charset);
			}
			catch
			{
			}

			return encoding ?? fallbackEncoding ?? Encoding.UTF8;
		}

		internal static string ToDurationString(this TimeSpan timeSpan)
		{
			static void AppendFormatIfNecessary(StringBuilder stringBuilder, string timePart, int value)
			{
				if (value <= 0) return;
				stringBuilder.AppendFormat($" {value} {timePart}{(value > 1 ? "s" : string.Empty)}");
			}

			if (timeSpan.TotalMilliseconds <= 1) return "1 millisecond";

			var result = new StringBuilder();
			AppendFormatIfNecessary(result, "day", timeSpan.Days);
			AppendFormatIfNecessary(result, "hour", timeSpan.Hours);
			AppendFormatIfNecessary(result, "minute", timeSpan.Minutes);
			AppendFormatIfNecessary(result, "second", timeSpan.Seconds);
			AppendFormatIfNecessary(result, "millisecond", timeSpan.Milliseconds);
			return result.ToString().Trim();
		}

		internal static string EnsureStartsWith(this string value, string prefix)
		{
			return !string.IsNullOrEmpty(value) && value.StartsWith(prefix) ? value : string.Concat(prefix, value);
		}

		internal static string EnsureEndsWith(this string value, string suffix)
		{
			return !string.IsNullOrEmpty(value) && value.EndsWith(suffix) ? value : string.Concat(value, suffix);
		}

		internal static JsonElement? GetProperty(this JsonElement element, string name, bool throwIfMissing)
		{
			var parts = name.Split('/');
			if (!element.TryGetProperty(parts[0], out var property))
			{
				if (throwIfMissing) throw new ArgumentException($"Unable to find '{name}'", nameof(name));
				return null;
			}

			foreach (var part in parts.Skip(1))
			{
				if (!property.TryGetProperty(part, out property))
				{
					if (throwIfMissing) throw new ArgumentException($"Unable to find '{name}'", nameof(name));
					return null;
				}
			}

			return property;
		}

		internal static T GetPropertyValue<T>(this JsonElement element, string name, T defaultValue)
		{
			return GetPropertyValue(element, new[] { name }, defaultValue, false);
		}

		internal static T GetPropertyValue<T>(this JsonElement element, string[] names, T defaultValue)
		{
			return GetPropertyValue(element, names, defaultValue, false);
		}

		internal static T GetPropertyValue<T>(this JsonElement element, string name)
		{
			return GetPropertyValue<T>(element, new[] { name }, default, true);
		}

		internal static T GetPropertyValue<T>(this JsonElement element, string[] names)
		{
			return GetPropertyValue<T>(element, names, default, true);
		}

		private static T GetPropertyValue<T>(JsonElement element, string[] names, T defaultValue, bool throwIfMissing)
		{
			JsonElement? property = null;
			foreach (var name in names)
			{
				property = element.GetProperty(name, false);
				if (property.HasValue) break;
			}

			if (!property.HasValue)
			{
				if (throwIfMissing) throw new Exception($"Unable to find {string.Join(", ", names)} in the Json document");
				return defaultValue;
			}

			var typeOfT = typeof(T);
			if (typeOfT.IsEnum)
			{
				return property.Value.ValueKind switch
				{
					JsonValueKind.String => (T)Enum.Parse(typeof(T), property.Value.GetString()),
					JsonValueKind.Number => (T)Enum.ToObject(typeof(T), property.Value.GetInt16()),
					_ => throw new ArgumentException($"Unable to convert a {property.Value.ValueKind} into a {typeof(T).FullName}", nameof(T))
				};
			}

			if (typeOfT.IsNullableType())
			{
				if (property.Value.ValueKind == JsonValueKind.Null) return default;
				var underlyingType = Nullable.GetUnderlyingType(typeOfT);
				var getElementValue = typeof(Extensions)
					.GetMethod(nameof(GetElementValue), BindingFlags.Static | BindingFlags.NonPublic)
					.MakeGenericMethod(underlyingType);
				return (T)getElementValue.Invoke(null, new object[] { property.Value });
			}

			if (typeOfT.IsArray)
			{
				if (property.Value.ValueKind == JsonValueKind.Null) return default;
				var elementType = typeOfT.GetElementType();
				var getElementValue = typeof(Extensions)
					.GetMethod(nameof(GetElementValue), BindingFlags.Static | BindingFlags.NonPublic)
					.MakeGenericMethod(elementType);
				var arrayList = new System.Collections.ArrayList(property.Value.GetArrayLength());
				foreach (var arrayElement in property.Value.EnumerateArray())
				{
					arrayList.Add(getElementValue.Invoke(null, new object[] { arrayElement }));
				}

				return (T)Convert.ChangeType(arrayList.ToArray(elementType), typeof(T));
			}

			return property.Value.GetElementValue<T>();
		}

		private static T GetElementValue<T>(this JsonElement element)
		{
			var typeOfT = typeof(T);
			if (element.ValueKind == JsonValueKind.Null)
			{
				return typeOfT.IsNullableType()
					? default
					: throw new Exception($"JSON contains a null value but {typeOfT.FullName} is not nullable");
			}

			return typeOfT switch
			{
				Type boolType when boolType == typeof(bool) => (T)(object)element.GetBoolean(),
				Type strType when strType == typeof(string) => (T)(object)element.GetString(),
				Type bytesType when bytesType == typeof(byte[]) => (T)(object)element.GetBytesFromBase64(),
				Type sbyteType when sbyteType == typeof(sbyte) => (T)(object)element.GetSByte(),
				Type byteType when byteType == typeof(byte) => (T)(object)element.GetByte(),
				Type shortType when shortType == typeof(short) => (T)(object)element.GetInt16(),
				Type ushortType when ushortType == typeof(ushort) => (T)(object)element.GetUInt16(),
				Type intType when intType == typeof(int) => (T)(object)element.GetInt32(),
				Type uintType when uintType == typeof(uint) => (T)(object)element.GetUInt32(),
				Type longType when longType == typeof(long) => (T)(object)element.GetInt64(),
				Type ulongType when ulongType == typeof(ulong) => (T)(object)element.GetUInt64(),
				Type doubleType when doubleType == typeof(double) => (T)(object)element.GetDouble(),
				Type floatType when floatType == typeof(float) => (T)(object)element.GetSingle(),
				Type decimalType when decimalType == typeof(decimal) => (T)(object)element.GetDecimal(),
				Type datetimeType when datetimeType == typeof(DateTime) => (T)(object)element.GetDateTime(),
				Type offsetType when offsetType == typeof(DateTimeOffset) => (T)(object)element.GetDateTimeOffset(),
				Type guidType when guidType == typeof(Guid) => (T)(object)element.GetGuid(),
				_ => throw new ArgumentException($"Unsable to map {typeof(T).FullName} to a corresponding JSON type", nameof(T))
			};
		}

		internal static Task<TResult[]> ForEachAsync<T, TResult>(this IEnumerable<T> items, Func<T, Task<TResult>> action) => ForEachAsync(items, action, DEFAULT_DEGREE_OF_PARALLELISM);
		internal static Task<TResult[]> ForEachAsync<T, TResult>(this IEnumerable<T> items, Func<T, int, Task<TResult>> action) => ForEachAsync(items, action, DEFAULT_DEGREE_OF_PARALLELISM);

		internal static async Task<TResult[]> ForEachAsync<T, TResult>(this IEnumerable<T> items, Func<T, Task<TResult>> action, int maxDegreeOfParalellism)
		{
			var allTasks = new List<Task<TResult>>();
			using var throttler = new SemaphoreSlim(initialCount: maxDegreeOfParalellism);
			foreach (var item in items)
			{
				await throttler.WaitAsync();
				allTasks.Add(Task.Run(async () =>
				{
					try { return await action(item).ConfigureAwait(false); }
					finally { throttler.Release(); }
				}));
			}

			return await Task.WhenAll(allTasks).ConfigureAwait(false);
		}

		internal static async Task<TResult[]> ForEachAsync<T, TResult>(this IEnumerable<T> items, Func<T, int, Task<TResult>> action, int maxDegreeOfParalellism)
		{
			var allTasks = new List<Task<TResult>>();
			using var throttler = new SemaphoreSlim(initialCount: maxDegreeOfParalellism);
			foreach (var (item, index) in items.Select((value, i) => (value, i)))
			{
				await throttler.WaitAsync();
				allTasks.Add(Task.Run(async () =>
				{
					try { return await action(item, index).ConfigureAwait(false); }
					finally { throttler.Release(); }
				}));
			}

			return await Task.WhenAll(allTasks).ConfigureAwait(false);
		}

		internal static Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> action) => ForEachAsync(items, action, DEFAULT_DEGREE_OF_PARALLELISM);
		internal static Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, int, Task> action) => ForEachAsync(items, action, DEFAULT_DEGREE_OF_PARALLELISM);

		internal static async Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> action, int maxDegreeOfParalellism)
		{
			var allTasks = new List<Task>();
			using var throttler = new SemaphoreSlim(initialCount: maxDegreeOfParalellism);
			foreach (var item in items)
			{
				await throttler.WaitAsync();
				allTasks.Add(Task.Run(async () =>
				{
					try { await action(item).ConfigureAwait(false); }
					finally { throttler.Release(); }
				}));
			}

			await Task.WhenAll(allTasks).ConfigureAwait(false);
		}

		internal static async Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, int, Task> action, int maxDegreeOfParalellism)
		{
			var allTasks = new List<Task>();
			using var throttler = new SemaphoreSlim(initialCount: maxDegreeOfParalellism);
			foreach (var (item, index) in items.Select((value, i) => (value, i)))
			{
				await throttler.WaitAsync();
				allTasks.Add(Task.Run(async () =>
				{
					try { await action(item, index).ConfigureAwait(false); }
					finally { throttler.Release(); }
				}));
			}

			await Task.WhenAll(allTasks).ConfigureAwait(false);
		}

		internal static T GetAttributeOfType<T>(this Enum enumVal)
			where T : Attribute
		{
			return enumVal.GetType()
				.GetTypeInfo()
				.DeclaredMembers
				.SingleOrDefault(x => x.Name == enumVal.ToString())
				?.GetCustomAttribute<T>(false);
		}

		internal static bool IsNumber(this object value)
		{
			return value is sbyte
				   || value is byte
				   || value is short
				   || value is ushort
				   || value is int
				   || value is uint
				   || value is long
				   || value is ulong
				   || value is float
				   || value is double
				   || value is decimal;
		}

		internal static string GetValue(this HttpHeaders headers, string name)
		{
			if (headers == null) return null;
			if (headers.TryGetValues(name, out IEnumerable<string> values)) return values.FirstOrDefault();
			return null;
		}

		internal static IEnumerable<KeyValuePair<string, string>> ParseQuerystring(this Uri uri)
		{
			return uri.Query.TrimStart('?')
				.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(value => value.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
				.Select(splitValue => splitValue.Length == 1
					? new KeyValuePair<string, string>(splitValue[0].Trim(), null)
					: new KeyValuePair<string, string>(splitValue[0].Trim(), splitValue[1].Trim()));
		}

		internal static async Task<Stream> CompressAsync(this Stream source)
		{
			var compressedStream = new MemoryStream();
			using (var gzip = new GZipStream(compressedStream, CompressionMode.Compress, true))
			{
				await source.CopyToAsync(gzip).ConfigureAwait(false);
			}

			compressedStream.Position = 0;
			return compressedStream;
		}

		internal static async Task<Stream> DecompressAsync(this Stream source)
		{
			var decompressedStream = new MemoryStream();
			using (var gzip = new GZipStream(source, CompressionMode.Decompress, true))
			{
				await gzip.CopyToAsync(decompressedStream).ConfigureAwait(false);
			}

			decompressedStream.Position = 0;
			return decompressedStream;
		}

		internal static string ToEnumString<T>(this T enumValue)
			where T : Enum
		{
			if (TryToEnumString(enumValue, out string stringValue)) return stringValue;
			return enumValue.ToString();
		}

		internal static bool TryToEnumString<T>(this T enumValue, out string stringValue)
			where T : Enum
		{
			var enumMemberAttribute = enumValue.GetAttributeOfType<EnumMemberAttribute>();
			if (enumMemberAttribute != null)
			{
				stringValue = enumMemberAttribute.Value;
				return true;
			}

			var jsonPropertyNameAttribute = enumValue.GetAttributeOfType<JsonPropertyNameAttribute>();
			if (jsonPropertyNameAttribute != null)
			{
				stringValue = jsonPropertyNameAttribute.Name;
				return true;
			}

			var descriptionAttribute = enumValue.GetAttributeOfType<DescriptionAttribute>();
			if (descriptionAttribute != null)
			{
				stringValue = descriptionAttribute.Description;
				return true;
			}

			stringValue = null;
			return false;
		}

		internal static T ToEnum<T>(this string str)
			where T : Enum
		{
			if (TryToEnum(str, out T enumValue)) return enumValue;
			throw new ArgumentException($"There is no value in the {typeof(T).Name} enum that corresponds to '{str}'.");
		}

		internal static bool TryToEnum<T>(this string str, out T enumValue)
			where T : Enum
		{
			var enumType = typeof(T);
			foreach (var name in Enum.GetNames(enumType))
			{
				var customAttributes = enumType.GetField(name).GetCustomAttributes(true);
				if (customAttributes.OfType<EnumMemberAttribute>().Any(attribute => string.Equals(attribute.Value, str, StringComparison.OrdinalIgnoreCase))
					|| customAttributes.OfType<JsonPropertyNameAttribute>().Any(attribute => string.Equals(attribute.Name, str, StringComparison.OrdinalIgnoreCase))
					|| customAttributes.OfType<DescriptionAttribute>().Any(attribute => string.Equals(attribute.Description, str, StringComparison.OrdinalIgnoreCase))
					|| string.Equals(name, str, StringComparison.OrdinalIgnoreCase))
				{
					enumValue = (T)Enum.Parse(enumType, name);
					return true;
				}
			}

			enumValue = default;
			return false;
		}

		internal static string ToHexString(this byte[] bytes)
		{
			var result = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; i++)
				result.Append(bytes[i].ToString("x2"));
			return result.ToString();
		}

		internal static string ToExactLength(this string source, int totalWidth, string postfix = "...", char paddingChar = ' ')
		{
			if (string.IsNullOrEmpty(source)) return new string(paddingChar, totalWidth);
			if (source.Length <= totalWidth) return source.PadRight(totalWidth, paddingChar);
			return $"{source.Substring(0, totalWidth - (postfix?.Length ?? 0))}{postfix ?? string.Empty}";
		}

		internal static bool IsNullableType(this Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		internal static string GetFriendlyName(this Type type)
		{
			return _typeAliases.TryGetValue(type, out var alias) ? alias : type.FullName;
		}
	}
}
