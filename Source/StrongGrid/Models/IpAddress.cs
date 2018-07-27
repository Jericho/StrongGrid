using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

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
		[JsonProperty("ip", NullValueHandling = NullValueHandling.Ignore)]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the subusers that are able to send email from this IP.
		/// </summary>
		/// <value>
		/// The subusers.
		/// </value>
		[JsonProperty("subusers", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Subusers { get; set; }

		/// <summary>
		/// Gets or sets the reverse DNS record for this IP address.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonProperty("rdns", NullValueHandling = NullValueHandling.Ignore)]
		public string ReverseDns { get; set; }

		/// <summary>
		/// Gets or sets the IP pools that this IP has been added to.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonProperty("pools", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Pools { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the IP address is currently warming up.
		/// </summary>
		/// <value>
		/// The warmup indicator.
		/// </value>
		[JsonProperty("warmup", NullValueHandling = NullValueHandling.Ignore)]
		public bool Warmup { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the IP address has been whitelabeled.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonProperty("whitelabeled", NullValueHandling = NullValueHandling.Ignore)]
		public bool WhiteLabeled { get; set; }

		/// <summary>
		/// Gets or sets the date that the IP address was entered into warmup.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonProperty("start_date", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime WarmupStartedOn { get; set; }

		/// <summary>
		/// Gets or sets the date that the IP address was assigned to the user.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonProperty("assigned_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime AssignedOn { get; set; }
	}
}
