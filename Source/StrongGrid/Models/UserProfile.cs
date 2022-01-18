using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// User profile.
	/// </summary>
	public class UserProfile
	{
		/// <summary>
		/// Gets or sets the address.
		/// </summary>
		/// <value>
		/// The address.
		/// </value>
		[JsonPropertyName("address")]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>
		/// The city.
		/// </value>
		[JsonPropertyName("city")]
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the company.
		/// </summary>
		/// <value>
		/// The company.
		/// </value>
		[JsonPropertyName("company")]
		public string Company { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		/// <value>
		/// The country.
		/// </value>
		[JsonPropertyName("country")]
		public string Country { get; set; }

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
		/// Gets or sets the phone.
		/// </summary>
		/// <value>
		/// The phone.
		/// </value>
		[JsonPropertyName("phone")]
		public string Phone { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
		[JsonPropertyName("state")]
		public string State { get; set; }

		/// <summary>
		/// Gets or sets the website.
		/// </summary>
		/// <value>
		/// The website.
		/// </value>
		[JsonPropertyName("website")]
		public string Website { get; set; }

		/// <summary>
		/// Gets or sets the zip code.
		/// </summary>
		/// <value>
		/// The zip code.
		/// </value>
		[JsonPropertyName("zip")]
		public string ZipCode { get; set; }
	}
}
