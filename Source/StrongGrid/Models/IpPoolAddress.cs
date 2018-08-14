using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about an IP address in an IP pool.
	/// </summary>
	public class IpPoolAddress
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
		/// Gets or sets the date that the IP address was entered into warmup.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonProperty("start_date", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime? WarmupStartedOn { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the IP address is currently warming up.
		/// </summary>
		/// <value>
		/// The warmup indicator.
		/// </value>
		[JsonProperty("warmup", NullValueHandling = NullValueHandling.Ignore)]
		public bool Warmup { get; set; }
	}
}
