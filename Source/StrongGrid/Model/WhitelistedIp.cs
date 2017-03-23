using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	/// <summary>
	///  IP that can access the user's account through the web, API, or mail send.
	/// </summary>
	public class WhitelistedIp
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the ip address.
		/// </summary>
		/// <value>
		/// The ip address.
		/// </value>
		[JsonProperty("ip", NullValueHandling = NullValueHandling.Ignore)]
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }
	}
}
