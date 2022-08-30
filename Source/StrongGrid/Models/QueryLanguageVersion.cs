using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the version of the query language.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<QueryLanguageVersion>))]
	public enum QueryLanguageVersion
	{
		/// <summary>Unspecified.</summary>
		/// <remarks>When a segment query version is unspecified, you can safely assume it's a Version1 query.</remarks>
		[EnumMember(Value = "")]
		Unspecified = 0,

		/// <summary>Version 1.</summary>
		/// <remarks>The v1 query language is documented <a href="https://docs.sendgrid.com/for-developers/sending-email/segmentation-query-language">here</a>.</remarks>
		[EnumMember(Value = "1")]
		Version1 = 1,

		/// <summary>Version 2.</summary>
		/// <remarks>The v2 query language is documented <a href="https://docs.sendgrid.com/for-developers/sending-email/marketing-campaigns-v2-segmentation-query-reference">here</a>.</remarks>
		[EnumMember(Value = "2")]
		Version2 = 2
	}
}
