using StrongGrid.Json;
using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// An invalid email address.
	/// </summary>
	public class InvalidEmail
	{
		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonPropertyName("email")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonPropertyName("reason")]
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonPropertyName("created")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }
	}
}
