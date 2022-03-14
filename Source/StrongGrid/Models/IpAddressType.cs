using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate if an IP address is dedicated or shared.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<IpAddressType>))]
	public enum IpAddressType
	{
		/// <summary>
		/// Dedicated IP address.
		/// </summary>
		[EnumMember(Value = "dedicated")]
		Dedicated,

		/// <summary>
		/// Shared IP address.
		/// </summary>
		[EnumMember(Value = "shared")]
		Shared
	}
}
