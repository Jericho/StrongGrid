using StrongGrid.Json;
using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about a suppression.
	/// </summary>
	public class Suppression
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
		/// Gets or sets the suppression group id.
		/// </summary>
		/// <value>
		/// The group id.
		/// </value>
		[JsonPropertyName("group_id")]
		public long GroupId { get; set; }

		/// <summary>
		/// Gets or sets the name of the suppression group.
		/// </summary>
		/// <value>
		/// The name of the suppression group.
		/// </value>
		[JsonPropertyName("group_name")]
		public string GroupName { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonPropertyName("created_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }
	}
}
