using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about a suppression
	/// </summary>
	public class Suppression
	{
		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the suppression group id.
		/// </summary>
		/// <value>
		/// The group id.
		/// </value>
		[JsonProperty("group_id", NullValueHandling = NullValueHandling.Ignore)]
		public long GroupId { get; set; }

		/// <summary>
		/// Gets or sets the name of the suppression group.
		/// </summary>
		/// <value>
		/// The name of the suppression group.
		/// </value>
		[JsonProperty("group_name", NullValueHandling = NullValueHandling.Ignore)]
		public string GroupName { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }
	}
}
