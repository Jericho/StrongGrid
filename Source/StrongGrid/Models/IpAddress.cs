using StrongGrid.Json;
using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about an IP address.
	/// </summary>
	public class IpAddress
	{
		/// <summary>
		/// Gets or sets the IP address.
		/// </summary>
		[JsonPropertyName("ip")]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the subusers that are able to send email from this IP.
		/// </summary>
		[JsonPropertyName("subusers")]
		public string[] Subusers { get; set; }

		/// <summary>
		/// Gets or sets the reverse DNS record for this IP address.
		/// </summary>
		[JsonPropertyName("rdns")]
		public string ReverseDns { get; set; }

		/// <summary>
		/// Gets or sets the IP pools that this IP has been added to.
		/// </summary>
		[JsonPropertyName("pools")]
		public string[] Pools { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the IP address is currently warming up.
		/// </summary>
		[JsonPropertyName("warmup")]
		public bool Warmup { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the IP address has been whitelabeled.
		/// </summary>
		[JsonPropertyName("whitelabeled")]
		public bool WhiteLabeled { get; set; }

		/// <summary>
		/// Gets or sets the date that the IP address was entered into warmup.
		/// </summary>
		[JsonPropertyName("start_date")]
		[JsonConverter(typeof(NullableEpochConverter))]
		public DateTime? WarmupStartedOn { get; set; }

		/// <summary>
		/// Gets or sets the date that the IP address was assigned to the user.
		/// </summary>
		[JsonPropertyName("assigned_at")]
		[JsonConverter(typeof(NullableEpochConverter))]
		public DateTime? AssignedOn { get; set; }
	}
}
