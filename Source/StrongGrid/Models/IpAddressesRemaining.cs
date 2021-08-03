using System.Text.Json.Serialization;

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
		[JsonPropertyName("remaining")]
		public int Remaining { get; set; }

		/// <summary>
		/// Gets or sets the length of time until user can add more IPs.
		/// </summary>
		/// <value>
		/// The length of time.
		/// </value>
		[JsonPropertyName("period")]
		public string Period { get; set; }

		/// <summary>
		/// Gets or sets the current cost to add an IP.
		/// </summary>
		/// <value>
		/// The cost.
		/// </value>
		[JsonPropertyName("price_per_ip")]
		public double PricePerIp { get; set; }
	}
}
