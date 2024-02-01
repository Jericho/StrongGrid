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
		/// <value>
		/// The IP address.
		/// </value>
		[JsonPropertyName("ip")]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the subusers that are able to send email from this IP.
		/// </summary>
		/// <value>
		/// The subusers.
		/// </value>
		[JsonPropertyName("subusers")]
		public string[] Subusers { get; set; }

		/// <summary>
		/// Gets or sets the reverse DNS record for this IP address.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonPropertyName("rdns")]
		public string ReverseDns { get; set; }

		/// <summary>
		/// Gets or sets the IP pools that this IP has been added to.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonPropertyName("pools")]
		public string[] Pools { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the IP address is currently warming up.
		/// </summary>
		/// <value>
		/// The warmup indicator.
		/// </value>
		[JsonPropertyName("warmup")]
		public bool Warmup { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the IP address has been whitelabeled.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonPropertyName("whitelabeled")]
		public bool WhiteLabeled { get; set; }

		/// <summary>
		/// Gets or sets the date that the IP address was entered into warmup.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonPropertyName("start_date")]
		[JsonConverter(typeof(NullableEpochConverter))]
		public DateTime? WarmupStartedOn { get; set; }

		/// <summary>
		/// Gets or sets the date that the IP address was assigned to the user.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonPropertyName("assigned_at")]
		[JsonConverter(typeof(NullableEpochConverter))]
		public DateTime? AssignedOn { get; set; }
	}
}
