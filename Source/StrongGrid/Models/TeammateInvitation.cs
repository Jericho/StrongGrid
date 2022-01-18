using System.Text.Json.Serialization;

namespace StrongGrid.Models
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
		[JsonPropertyName("pending_id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonPropertyName("email")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the scopes.
		/// </summary>
		/// <value>
		/// The scopes.
		/// </value>
		[JsonPropertyName("scopes")]
		public string[] Scopes { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a teammate has admin permissions.
		/// </summary>
		/// <value>
		/// The flag that indicates if a teammate has admin permissions.
		/// </value>
		[JsonPropertyName("is_admin")]
		public bool IsAdmin { get; set; }
	}
}
