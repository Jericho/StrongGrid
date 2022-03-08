using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Enumeration to indicate the type of url.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<UrlType>))]
	public enum UrlType
	{
		/// <summary>
		/// Text.
		/// </summary>
		[EnumMember(Value = "text")]
		Text,

		/// <summary>
		/// HTML.
		/// </summary>
		[EnumMember(Value = "html")]
		Html,

		/// <summary>
		/// AMP.
		/// </summary>
		[EnumMember(Value = "amp")]
		Amp,

		/// <summary>
		/// Header.
		/// </summary>
		[EnumMember(Value = "header")]
		Header
	}
}
