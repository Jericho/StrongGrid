using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

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
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

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
		/// Gets or sets the last clicked on.
		/// </summary>
		/// <value>
		/// The last clicked on.
		/// </value>
		[JsonProperty("last_clicked", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime? LastClickedOn { get; set; }

		/// <summary>
		/// Gets or sets the last emailed on.
		/// </summary>
		/// <value>
		/// The last emailed on.
		/// </value>
		[JsonProperty("last_emailed", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime? LastEmailedOn { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		[JsonProperty("last_name", NullValueHandling = NullValueHandling.Ignore)]
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the last opened on.
		/// </summary>
		/// <value>
		/// The last opened on.
		/// </value>
		[JsonProperty("last_opened", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime? LastOpenedOn { get; set; }

		/// <summary>
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }

		/// <summary>
		/// Gets or sets the custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		[JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(LegacyCustomFieldsConverter))]
		public Field[] CustomFields { get; set; }
	}
}
