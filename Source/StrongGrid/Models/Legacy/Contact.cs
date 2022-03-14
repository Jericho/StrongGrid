using StrongGrid.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Legacy
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
			CustomFields = customFields.ToArray() ?? Array.Empty<Field>();
		}

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonPropertyName("created_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

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
		/// Gets or sets the last clicked on.
		/// </summary>
		/// <value>
		/// The last clicked on.
		/// </value>
		[JsonPropertyName("last_clicked")]
		[JsonConverter(typeof(NullableEpochConverter))]
		public DateTime? LastClickedOn { get; set; }

		/// <summary>
		/// Gets or sets the last emailed on.
		/// </summary>
		/// <value>
		/// The last emailed on.
		/// </value>
		[JsonPropertyName("last_emailed")]
		[JsonConverter(typeof(NullableEpochConverter))]
		public DateTime? LastEmailedOn { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		[JsonPropertyName("last_name")]
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the last opened on.
		/// </summary>
		/// <value>
		/// The last opened on.
		/// </value>
		[JsonPropertyName("last_opened")]
		[JsonConverter(typeof(NullableEpochConverter))]
		public DateTime? LastOpenedOn { get; set; }

		/// <summary>
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonPropertyName("updated_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }

		/// <summary>
		/// Gets or sets the custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		[JsonPropertyName("custom_fields")]
		[JsonConverter(typeof(LegacyCustomFieldsConverter))]
		public Field[] CustomFields { get; set; }
	}
}
