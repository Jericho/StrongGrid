using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Models
{
	/// <summary>
	/// Activity Entry.
	/// </summary>
	public class AccessEntry
	{
		/// <summary>
		/// Gets or sets a value indicating whether or not access was granted.
		/// </summary>
		/// <value>
		///   <c>true</c> if access was granted; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("allowed", NullValueHandling = NullValueHandling.Ignore)]
		public bool Allowed { get; set; }

		/// <summary>
		/// Gets or sets the authorization method.
		/// </summary>
		/// <value>
		/// The authorization method.
		/// </value>
		[JsonProperty("auth_method", NullValueHandling = NullValueHandling.Ignore)]
		public string AuthorizationMethod { get; set; }

		/// <summary>
		/// Gets or sets the date this IP was fist used to access the account.
		/// </summary>
		/// <value>
		/// The date.
		/// </value>
		[JsonProperty("first_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime FirstAccessOn { get; set; }

		/// <summary>
		/// Gets or sets the ip address.
		/// </summary>
		/// <value>
		/// The ip address.
		/// </value>
		[JsonProperty("ip", NullValueHandling = NullValueHandling.Ignore)]
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the date this IP was most recently used to access the account.
		/// </summary>
		/// <value>
		/// The user name.
		/// </value>
		[JsonProperty("last_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime LatestAccessOn { get; set; }

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>
		/// The location.
		/// </value>
		[JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
		public string Location { get; set; }
	}
}
