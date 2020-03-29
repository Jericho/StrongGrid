using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Enumeration to indicate the filter field when searching for contacts.
	/// </summary>
	public enum ContactsFilterField
	{
		/// <summary>
		/// The email address.
		/// </summary>
		[EnumMember(Value = "email")]
		EmailAddress,

		/// <summary>
		/// The email domains.
		/// </summary>
		[EnumMember(Value = "email_domains")]
		EmailDomains,

		/// <summary>
		/// The first name.
		/// </summary>
		[EnumMember(Value = "first_name")]
		FirstName,

		/// <summary>
		/// The last name.
		/// </summary>
		[EnumMember(Value = "last_name")]
		LastName,

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
		/// The state/province/region.
		/// </summary>
		[EnumMember(Value = "state_province_region")]
		StateOrProvice,

		/// <summary>
		/// The country.
		/// </summary>
		[EnumMember(Value = "country")]
		Country,

		/// <summary>
		/// The postal code.
		/// </summary>
		[EnumMember(Value = "postal_code")]
		PostalCode,
	}
}
