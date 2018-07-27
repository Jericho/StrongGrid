using Newtonsoft.Json;

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
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the scope group name.
		/// </summary>
		/// <value>
		/// The scope.
		/// </value>
		[JsonProperty("scope_group_name", NullValueHandling = NullValueHandling.Ignore)]
		public string ScopeGroupName { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The user name.
		/// </value>
		[JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		/// <value>
		/// The first name.
		/// </value>
		[JsonProperty("first_name", NullValueHandling = NullValueHandling.Ignore)]
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		[JsonProperty("lastname", NullValueHandling = NullValueHandling.Ignore)]
		public string LastName { get; set; }
	}
}
