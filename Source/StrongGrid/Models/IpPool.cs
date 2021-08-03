using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about an IP pool.
	/// </summary>
	public class IpPool
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonPropertyName("pool_name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the IP addresses in the IP pool.
		/// </summary>
		/// <value>
		/// The IP addresses.
		/// </value>
		[JsonPropertyName("ips")]
		public IpPoolAddress[] IpAddresses { get; set; }
	}
}
