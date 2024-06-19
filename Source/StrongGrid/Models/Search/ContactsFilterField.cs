using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Enumeration to indicate the filter field when searching for contacts.
	/// </summary>
	public enum ContactsFilterField
	{
		/// <summary>
		/// The additional emails associated with the contact.
		/// </summary>
		[EnumMember(Value = "alternate_emails")]
		AlternateEmails,

		/// <summary>
		/// The address line 1.
		/// </summary>
		[EnumMember(Value = "address_line_1")]
		AddresLine1,

		/// <summary>
		/// The address line 2.
		/// </summary>
		[EnumMember(Value = "address_line_2")]
		AddresLine2,

		/// <summary>
		/// The city.
		/// </summary>
		[EnumMember(Value = "city")]
		City,

		/// <summary>
		/// The contact identifier.
		/// </summary>
		[EnumMember(Value = "contact_id")]
		Id,

		/// <summary>
		/// The country.
		/// </summary>
		[EnumMember(Value = "country")]
		Country,

		/// <summary>
		/// The date the contact was created.
		/// </summary>
		[EnumMember(Value = "created_at")]
		CreatedOn,

		/// <summary>
		/// The email address.
		/// </summary>
		[EnumMember(Value = "email")]
		EmailAddress,

		/// <summary>
		/// The phone number.
		/// </summary>
		[EnumMember(Value = "phone_number_id")]
		PhoneNumberId,

		/// <summary>
		/// The external id.
		/// </summary>
		[EnumMember(Value = "external_id")]
		ExternalId,

		/// <summary>
		/// The anonymous id.
		/// </summary>
		[EnumMember(Value = "anonymous_id")]
		AnonymousId,

		/// <summary>
		/// The email domains.
		/// </summary>
		[EnumMember(Value = "email_domains")]
		EmailDomains,

		/// <summary>
		/// No documentation has been provided.
		/// See &lt;a href="https://docs.sendgrid.com/for-developers/sending-email/segmentation-query-language#fields"&gt;SendGrid documentation&lt;a&gt;.
		/// </summary>
		[EnumMember(Value = "event_data")]
		EventData,

		/// <summary>
		/// No documentation has been provided.
		/// See &lt;a href="https://docs.sendgrid.com/for-developers/sending-email/segmentation-query-language#fields"&gt;SendGrid documentation&lt;a&gt;.
		/// </summary>
		[EnumMember(Value = "event_source")]
		EventSource,

		/// <summary>
		/// No documentation has been provided.
		/// See &lt;a href="https://docs.sendgrid.com/for-developers/sending-email/segmentation-query-language#fields"&gt;SendGrid documentation&lt;a&gt;.
		/// </summary>
		[EnumMember(Value = "event_timestamp")]
		EventTimestamp,

		/// <summary>
		/// No documentation has been provided.
		/// See &lt;a href="https://docs.sendgrid.com/for-developers/sending-email/segmentation-query-language#fields"&gt;SendGrid documentation&lt;a&gt;.
		/// </summary>
		[EnumMember(Value = "event_type")]
		EventType,

		/// <summary>
		/// The first name.
		/// </summary>
		[EnumMember(Value = "first_name")]
		FirstName,

		/// <summary>
		/// The lists associated with this contact.
		/// </summary>
		[EnumMember(Value = "list_ids")]
		ListIds,

		/// <summary>
		/// The last name.
		/// </summary>
		[EnumMember(Value = "last_name")]
		LastName,

		/// <summary>
		/// The postal code.
		/// </summary>
		[EnumMember(Value = "postal_code")]
		PostalCode,

		/// <summary>
		/// The state/province/region.
		/// </summary>
		[EnumMember(Value = "state_province_region")]
		StateOrProvice,

		/// <summary>
		/// The date the contact was last modified.
		/// </summary>
		[EnumMember(Value = "updated_at")]
		ModifiedOn
	}
}
