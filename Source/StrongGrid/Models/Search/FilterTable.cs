using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Enumeration to indicate the filter table.
	/// </summary>
	public enum FilterTable
	{
		/// <summary>
		/// Unspecified.
		/// </summary>
		[EnumMember(Value = "")]
		Unspecified,

		/// <summary>
		/// Contacts.
		/// </summary>
		[EnumMember(Value = "contact_data")]
		Contacts,

		/// <summary>
		/// Events.
		/// </summary>
		[EnumMember(Value = "event_data")]
		Events,

		/// <summary>
		/// Email activities.
		/// </summary>
		[EnumMember(Value = "email_activities")]
		EmailActivities
	}
}
