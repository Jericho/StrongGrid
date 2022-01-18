using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Access request details.
	/// </summary>
	public class AccessRequest
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the scope group name.
		/// </summary>
		/// <value>
		/// The scope.
		/// </value>
		[JsonPropertyName("scope_group_name")]
		public string ScopeGroupName { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The user name.
		/// </value>
		[JsonPropertyName("username")]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonPropertyName("email")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		/// <value>
		/// The first name.
		/// </value>
		[JsonPropertyName("first_name")]
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		[JsonPropertyName("lastname")]
		public string LastName { get; set; }
	}
}
