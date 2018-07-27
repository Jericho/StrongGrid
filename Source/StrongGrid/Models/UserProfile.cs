using Newtonsoft.Json;

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
		[JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>
		/// The city.
		/// </value>
		[JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the company.
		/// </summary>
		/// <value>
		/// The company.
		/// </value>
		[JsonProperty("company", NullValueHandling = NullValueHandling.Ignore)]
		public string Company { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		/// <value>
		/// The country.
		/// </value>
		[JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
		public string Country { get; set; }

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
		/// Gets or sets the phone.
		/// </summary>
		/// <value>
		/// The phone.
		/// </value>
		[JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
		public string Phone { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
		[JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
		public string State { get; set; }

		/// <summary>
		/// Gets or sets the website.
		/// </summary>
		/// <value>
		/// The website.
		/// </value>
		[JsonProperty("website", NullValueHandling = NullValueHandling.Ignore)]
		public string Website { get; set; }

		/// <summary>
		/// Gets or sets the zip code.
		/// </summary>
		/// <value>
		/// The zip code.
		/// </value>
		[JsonProperty("zip", NullValueHandling = NullValueHandling.Ignore)]
		public string ZipCode { get; set; }
	}
}
