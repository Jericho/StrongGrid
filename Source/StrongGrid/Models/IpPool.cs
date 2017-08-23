using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about an IP pool
	/// </summary>
	public class IpPool
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the IP addresses in the IP pool.
		/// </summary>
		/// <value>
		/// The IP addresses.
		/// </value>
		[JsonProperty("ips", NullValueHandling = NullValueHandling.Ignore)]
		public string[] IpAddresses { get; set; }
	}
}
