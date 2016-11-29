using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	/// <summary>
	/// An invalid email address
	/// </summary>
	public class InvalidEmail
	{
		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonProperty("email")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("reason")]
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonProperty("created")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }
	}
}
