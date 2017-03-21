using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Teammate.
	/// </summary>
	public class TeammateInvitation
	{
		/// <summary>
		/// Gets or sets the invitation ID.
		/// </summary>
		/// <value>
		/// The ID.
		/// </value>
		[JsonProperty("pending_id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the scopes.
		/// </summary>
		/// <value>
		/// The scopes.
		/// </value>
		[JsonProperty("scopes", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Scopes { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a teammate has admin permissions.
		/// </summary>
		/// <value>
		/// The flag that indicates if a teammate has admin permissions.
		/// </value>
		[JsonProperty("is_admin", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsAdmin { get; set; }
	}
}
