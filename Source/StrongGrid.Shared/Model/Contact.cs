using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrongGrid.Model
{
	public class Contact
	{
		public Contact()
		{
		}

		public Contact(string email, string firstName = null, string lastName = null, IEnumerable<Field> customFields = null)
		{
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			CustomFields = (customFields ?? Enumerable.Empty<Field>()).ToArray();
		}

		[JsonProperty("created_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("last_clicked")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime? LastClickedOn { get; set; }

		[JsonProperty("last_emailed")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime? LastEmailedOn { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		[JsonProperty("last_opened")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime? LastOpenedOn { get; set; }

		[JsonProperty("updated_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }

		[JsonProperty("custom_fields")]
		[JsonConverter(typeof(CustomFieldsConverter))]
		public Field[] CustomFields { get; set; }
	}
}
