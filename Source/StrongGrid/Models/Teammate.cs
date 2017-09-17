using Newtonsoft.Json;

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
		[JsonProperty("last_name", NullValueHandling = NullValueHandling.Ignore)]
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the user type.
		/// </summary>
		/// <value>
		/// The user type.
		/// </value>
		[JsonProperty("user_type", NullValueHandling = NullValueHandling.Ignore)]
		public string UserType { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a teammate has admin permissions.
		/// </summary>
		/// <value>
		/// The flag that indicates if a teammate has admin permissions.
		/// </value>
		[JsonProperty("is_admin", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsAdmin { get; set; }

		/// <summary>
		/// Gets or sets the phone.
		/// </summary>
		/// <value>
		/// The phone.
		/// </value>
		[JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
		public string Phone { get; set; }

		/// <summary>
		/// Gets or sets the website.
		/// </summary>
		/// <value>
		/// The website.
		/// </value>
		[JsonProperty("website", NullValueHandling = NullValueHandling.Ignore)]
		public string Website { get; set; }

		/// <summary>
		/// Gets or sets the company.
		/// </summary>
		/// <value>
		/// The company.
		/// </value>
		[JsonProperty("company", NullValueHandling = NullValueHandling.Ignore)]
		public string Company { get; set; }

		/// <summary>
		/// Gets or sets the address line 1.
		/// </summary>
		/// <value>
		/// The address line 1.
		/// </value>
		[JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
		public string AddressLine1 { get; set; }

		/// <summary>
		/// Gets or sets the address line 2.
		/// </summary>
		/// <value>
		/// The address line 2.
		/// </value>
		[JsonProperty("address2", NullValueHandling = NullValueHandling.Ignore)]
		public string AddressLine2 { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>
		/// The city.
		/// </value>
		[JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
		[JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
		public string State { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		/// <value>
		/// The country.
		/// </value>
		[JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
		public string Country { get; set; }

		/// <summary>
		/// Gets or sets the zip code.
		/// </summary>
		/// <value>
		/// The zip code.
		/// </value>
		[JsonProperty("zip", NullValueHandling = NullValueHandling.Ignore)]
		public string ZipCode { get; set; }

		/// <summary>
		/// Gets or sets the scopes.
		/// </summary>
		/// <value>
		/// The scopes.
		/// </value>
		[JsonProperty("scopes", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Scopes { get; set; }
	}
}
