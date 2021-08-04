using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Add IP Address result.
	/// </summary>
	public class AddIpAddressResult
	{
		/// <summary>
		/// Gets or sets the IP Addresses.
		/// </summary>
		/// <remarks>
		/// Please note that SendGrid only returns the address of the new IPs as well as the array of users that the new addresses are assigned to.
		/// None of the other properties on the <see cref="IpAddress">Ip Addresses</see> are populated.
		/// </remarks>
		/// <value>
		/// An array of <see cref="IpAddress">IP Addresses</see>.
		/// </value>
		[JsonPropertyName("ips")]
		public IpAddress[] IpAddresses { get; set; }

		/// <summary>
		/// Gets or sets the number of IPs that can still be added to the user.
		/// </summary>
		/// <value>
		/// The number of IP addresses.
		/// </value>
		[JsonPropertyName("remaining_ips")]
		public int RemainingIpAddresses { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the IPs are being warmed up.
		/// </summary>
		/// <value>
		/// The value indicating whether or not the IPs are being warmed up.
		/// </value>
		[JsonPropertyName("warmup")]
		public bool WarmingUp { get; set; }
	}
}
