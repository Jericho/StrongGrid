using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Subusers are sendgrid users which exist as children of the main parent account.
	/// </summary>
	public class Subuser
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		[JsonPropertyName("id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The username.
		/// </value>
		[JsonPropertyName("username")]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		[JsonPropertyName("password")]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the email address.
		/// </summary>
		/// <value>
		/// The email address.
		/// </value>
		[JsonPropertyName("email")]
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the subuser is disabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is disabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("disabled")]
		public bool IsDisabled { get; set; }

		/// <summary>
		/// Gets or sets the ip addresses allocated to this subuser.
		/// </summary>
		/// <value>
		/// The ip addresses.
		/// </value>
		[JsonPropertyName("ips")]
		public string[] Ips { get; set; }
	}
}
