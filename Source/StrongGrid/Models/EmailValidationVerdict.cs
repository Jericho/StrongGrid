using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the verdict of an email validation.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum EmailValidationVerdict
	{
		/// <summary>
		/// Email address appears to be valid.
		/// </summary>
		Valid,

		/// <summary>
		/// Email address appears to be risky.
		/// </summary>
		[EnumMember(Value = "Risky")]
		Risky,

		/// <summary>
		/// Email address appears to be invalid.
		/// </summary>
		[EnumMember(Value = "Invalid")]
		Invalid
	}
}
