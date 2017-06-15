using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Enumeration to indicate the type of url
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum UrlType
	{
		/// <summary>
		/// Text
		/// </summary>
		[EnumMember(Value = "text")]
		Text,

		/// <summary>
		/// HTML
		/// </summary>
		[EnumMember(Value = "html")]
		Html,

		/// <summary>
		/// Header
		/// </summary>
		[EnumMember(Value = "header")]
		Header
	}
}
