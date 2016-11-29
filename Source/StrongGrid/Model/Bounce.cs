using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	/// <summary>
	/// Information about an email address that bounced
	/// </summary>
	public class Bounce
	{
		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonProperty("created")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the email address.
		/// </summary>
		/// <value>
		/// The email address.
		/// </value>
		[JsonProperty("email")]
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("reason")]
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonProperty("status")]
		public string Status { get; set; }
	}
}
