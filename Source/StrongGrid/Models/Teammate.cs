using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Teammate.
	/// </summary>
	public class Teammate
	{
		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The username.
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
		[JsonPropertyName("last_name")]
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the user type.
		/// </summary>
		/// <value>
		/// The user type.
		/// </value>
		[JsonPropertyName("user_type")]
		public string UserType { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a teammate has admin permissions.
		/// </summary>
		/// <value>
		/// The flag that indicates if a teammate has admin permissions.
		/// </value>
		[JsonPropertyName("is_admin")]
		public bool IsAdmin { get; set; }

		/// <summary>
		/// Gets or sets the phone.
		/// </summary>
		/// <value>
		/// The phone.
		/// </value>
		[JsonPropertyName("phone")]
		public string Phone { get; set; }

		/// <summary>
		/// Gets or sets the website.
		/// </summary>
		/// <value>
		/// The website.
		/// </value>
		[JsonPropertyName("website")]
		public string Website { get; set; }

		/// <summary>
		/// Gets or sets the company.
		/// </summary>
		/// <value>
		/// The company.
		/// </value>
		[JsonPropertyName("company")]
		public string Company { get; set; }

		/// <summary>
		/// Gets or sets the address line 1.
		/// </summary>
		/// <value>
		/// The address line 1.
		/// </value>
		[JsonPropertyName("address")]
		public string AddressLine1 { get; set; }

		/// <summary>
		/// Gets or sets the address line 2.
		/// </summary>
		/// <value>
		/// The address line 2.
		/// </value>
		[JsonPropertyName("address2")]
		public string AddressLine2 { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>
		/// The city.
		/// </value>
		[JsonPropertyName("city")]
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
		[JsonPropertyName("state")]
		public string State { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		/// <value>
		/// The country.
		/// </value>
		[JsonPropertyName("country")]
		public string Country { get; set; }

		/// <summary>
		/// Gets or sets the zip code.
		/// </summary>
		/// <value>
		/// The zip code.
		/// </value>
		[JsonPropertyName("zip")]
		public string ZipCode { get; set; }

		/// <summary>
		/// Gets or sets the scopes.
		/// </summary>
		/// <value>
		/// The scopes.
		/// </value>
		[JsonPropertyName("scopes")]
		public string[] Scopes { get; set; }
	}
}
