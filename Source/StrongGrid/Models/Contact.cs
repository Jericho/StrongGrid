using StrongGrid.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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
		[JsonPropertyName("address_line_1")]
		public string AddressLine1 { get; set; }

		/// <summary>
		/// Gets or sets the address line 2.
		/// </summary>
		/// <value>
		/// The address line 2.
		/// </value>
		[JsonPropertyName("address_line_2")]
		public string AddressLine2 { get; set; }

		/// <summary>
		/// Gets or sets the additional emails associated with the contact.
		/// </summary>
		/// <value>
		/// The additional emails associated with the contact.
		/// </value>
		[JsonPropertyName("alternate_emails")]

		public string[] AlternateEmails { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>
		/// The city.
		/// </value>
		[JsonPropertyName("city")]
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		/// <value>
		/// The country.
		/// </value>
		[JsonPropertyName("country")]
		public string Country { get; set; }

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
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		[JsonPropertyName("last_name")]
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the postal code.
		/// </summary>
		/// <value>
		/// The postal code.
		/// </value>
		[JsonPropertyName("postal_code")]
		public string PostalCode { get; set; }

		/// <summary>
		/// Gets or sets the state or province.
		/// </summary>
		/// <value>
		/// The state or province.
		/// </value>
		[JsonPropertyName("state_province_region")]
		public string StateOrProvice { get; set; }

		/// <summary>
		/// Gets or sets the unique identifiers of the lists that this contact is associated with.
		/// </summary>
		/// <value>
		/// The lists associated with this contact.
		/// </value>
		[JsonPropertyName("list_ids")]
		public string[] Lists { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonPropertyName("created_at")]
		[JsonConverter(typeof(SendGridDateTimeConverter))]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonPropertyName("updated_at")]
		[JsonConverter(typeof(SendGridDateTimeConverter))]
		public DateTime ModifiedOn { get; set; }

		/// <summary>
		/// Gets or sets the custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		[JsonPropertyName("custom_fields")]
		[JsonConverter(typeof(CustomFieldsConverter))]
		public Field[] CustomFields { get; set; }

		/// <summary>
		/// Gets or sets the phone number.
		/// </summary>
		/// <value>
		/// The phone number.
		/// </value>
		[JsonPropertyName("phone_number")]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// Gets or sets the whatsapp.
		/// </summary>
		/// <value>
		/// The whatsapp.
		/// </value>
		[JsonPropertyName("whatsapp")]
		public string WhatsApp { get; set; }

		/// <summary>
		/// Gets or sets the line.
		/// </summary>
		/// <value>
		/// The line.
		/// </value>
		[JsonPropertyName("line")]
		public string Line { get; set; }

		/// <summary>
		/// Gets or sets the facebook.
		/// </summary>
		/// <value>
		/// The facebook.
		/// </value>
		[JsonPropertyName("facebook")]
		public string Facebook { get; set; }

		/// <summary>
		/// Gets or sets the unique name.
		/// </summary>
		/// <value>
		/// The unique name.
		/// </value>
		[JsonPropertyName("unique_name")]
		public string UniqueName { get; set; }

		/// <summary>
		/// Gets or sets the last clicked on.
		/// </summary>
		/// <value>
		/// The last clicked on.
		/// </value>
		[JsonPropertyName("last_clicked")]
		public DateTime? LastClickedOn { get; set; }

		/// <summary>
		/// Gets or sets the last emailed on.
		/// </summary>
		/// <value>
		/// The last emailed on.
		/// </value>
		[JsonPropertyName("last_emailed")]
		public DateTime? LastEmailedOn { get; set; }

		/// <summary>
		/// Gets or sets the last opened on.
		/// </summary>
		/// <value>
		/// The last opened on.
		/// </value>
		[JsonPropertyName("last_opened")]
		public DateTime? LastOpenedOn { get; set; }
	}
}
