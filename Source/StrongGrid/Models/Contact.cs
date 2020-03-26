using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrongGrid.Models
{
	/// <summary>
	/// A contact (also known as a recipient).
	/// </summary>
	public class Contact
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Contact"/> class.
		/// </summary>
		public Contact()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Contact"/> class.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="customFields">The custom fields.</param>
		public Contact(string email, string firstName = null, string lastName = null, IEnumerable<Field> customFields = null)
		{
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			CustomFields = customFields?.ToArray() ?? Array.Empty<Field>();
		}

		/// <summary>
		/// Gets or sets the address line 1.
		/// </summary>
		/// <value>
		/// The address line 1.
		/// </value>
		[JsonProperty("address_line_1", NullValueHandling = NullValueHandling.Ignore)]
		public string AddressLine1 { get; set; }

		/// <summary>
		/// Gets or sets the address line 2.
		/// </summary>
		/// <value>
		/// The address line 2.
		/// </value>
		[JsonProperty("address_line_2", NullValueHandling = NullValueHandling.Ignore)]
		public string AddressLine2 { get; set; }

		/// <summary>
		/// Gets or sets the additional emails associated with the contact.
		/// </summary>
		/// <value>
		/// The additional emails associated with the contact.
		/// </value>
		[JsonProperty("alternate_emails", NullValueHandling = NullValueHandling.Ignore)]

		public string[] AlternateEmails { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>
		/// The city.
		/// </value>
		[JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		/// <value>
		/// The country.
		/// </value>
		[JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
		public string Country { get; set; }

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
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		[JsonProperty("last_name", NullValueHandling = NullValueHandling.Ignore)]
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the postal code.
		/// </summary>
		/// <value>
		/// The postal code.
		/// </value>
		[JsonProperty("postal_code", NullValueHandling = NullValueHandling.Ignore)]
		public string PostalCode { get; set; }

		/// <summary>
		/// Gets or sets the state or province.
		/// </summary>
		/// <value>
		/// The state or province.
		/// </value>
		[JsonProperty("state_province_region", NullValueHandling = NullValueHandling.Ignore)]
		public string StateOrProvice { get; set; }

		/// <summary>
		/// Gets or sets the unique identifiers of the lists that this contact is associated with.
		/// </summary>
		/// <value>
		/// The lists associated with this contact.
		/// </value>
		[JsonProperty("list_ids", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Lists { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime ModifiedOn { get; set; }

		/// <summary>
		/// Gets or sets the custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		[JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(CustomFieldsConverter))]
		public Field[] CustomFields { get; set; }

		/// <summary>
		/// Gets or sets the phone number.
		/// </summary>
		/// <value>
		/// The phone number.
		/// </value>
		[JsonProperty("phone_number", NullValueHandling = NullValueHandling.Ignore)]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// Gets or sets the whatsapp.
		/// </summary>
		/// <value>
		/// The whatsapp.
		/// </value>
		[JsonProperty("whatsapp", NullValueHandling = NullValueHandling.Ignore)]
		public string WhatsApp { get; set; }

		/// <summary>
		/// Gets or sets the line.
		/// </summary>
		/// <value>
		/// The line.
		/// </value>
		[JsonProperty("line", NullValueHandling = NullValueHandling.Ignore)]
		public string Line { get; set; }

		/// <summary>
		/// Gets or sets the facebook.
		/// </summary>
		/// <value>
		/// The facebook.
		/// </value>
		[JsonProperty("facebook", NullValueHandling = NullValueHandling.Ignore)]
		public string Facebook { get; set; }

		/// <summary>
		/// Gets or sets the unique name.
		/// </summary>
		/// <value>
		/// The unique name.
		/// </value>
		[JsonProperty("unique_name", NullValueHandling = NullValueHandling.Ignore)]
		public string UniqueName { get; set; }

		/// <summary>
		/// Gets or sets the last clicked on.
		/// </summary>
		/// <value>
		/// The last clicked on.
		/// </value>
		[JsonProperty("last_clicked", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? LastClickedOn { get; set; }

		/// <summary>
		/// Gets or sets the last emailed on.
		/// </summary>
		/// <value>
		/// The last emailed on.
		/// </value>
		[JsonProperty("last_emailed", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? LastEmailedOn { get; set; }

		/// <summary>
		/// Gets or sets the last opened on.
		/// </summary>
		/// <value>
		/// The last opened on.
		/// </value>
		[JsonProperty("last_opened", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? LastOpenedOn { get; set; }
	}
}
