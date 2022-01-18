using StrongGrid.Utilities;
using System;
using System.Text.Json.Serialization;

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
		[JsonPropertyName("allowed")]
		public bool Allowed { get; set; }

		/// <summary>
		/// Gets or sets the authorization method.
		/// </summary>
		/// <value>
		/// The authorization method.
		/// </value>
		[JsonPropertyName("auth_method")]
		public string AuthorizationMethod { get; set; }

		/// <summary>
		/// Gets or sets the date this IP was fist used to access the account.
		/// </summary>
		/// <value>
		/// The date.
		/// </value>
		[JsonPropertyName("first_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime FirstAccessOn { get; set; }

		/// <summary>
		/// Gets or sets the ip address.
		/// </summary>
		/// <value>
		/// The ip address.
		/// </value>
		[JsonPropertyName("ip")]
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the date this IP was most recently used to access the account.
		/// </summary>
		/// <value>
		/// The user name.
		/// </value>
		[JsonPropertyName("last_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime LatestAccessOn { get; set; }

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>
		/// The location.
		/// </value>
		[JsonPropertyName("location")]
		public string Location { get; set; }
	}
}
