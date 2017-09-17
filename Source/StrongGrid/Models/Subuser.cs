using Newtonsoft.Json;

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
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The username.
		/// </value>
		[JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		[JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the email address.
		/// </summary>
		/// <value>
		/// The email address.
		/// </value>
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the subuser is disabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is disabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("disabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsDisabled { get; set; }

		/// <summary>
		/// Gets or sets the ip addresses allocated to this subuser.
		/// </summary>
		/// <value>
		/// The ip addresses.
		/// </value>
		[JsonProperty("ips", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Ips { get; set; }
	}
}
