using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about remaining IP addresses.
	/// </summary>
	public class IpAddressesRemaining
	{
		/// <summary>
		/// Gets or sets the number of IPs that can still be added to the user.
		/// </summary>
		/// <value>
		/// The number of remaining addresses.
		/// </value>
		[JsonProperty("remaining", NullValueHandling = NullValueHandling.Ignore)]
		public int Remaining { get; set; }

		/// <summary>
		/// Gets or sets the length of time until user can add more IPs.
		/// </summary>
		/// <value>
		/// The length of time.
		/// </value>
		[JsonProperty("period", NullValueHandling = NullValueHandling.Ignore)]
		public string Period { get; set; }

		/// <summary>
		/// Gets or sets the current cost to add an IP.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonProperty("price_per_ip", NullValueHandling = NullValueHandling.Ignore)]
		public double PricePerIp { get; set; }
	}
}
