using StrongGrid.Resources;

namespace StrongGrid
{
	/// <summary>
	/// Interface for the StrongGrid REST client for communicating with SendGrid's API.
	/// </summary>
	public interface IStrongGridClient
	{
		/// <summary>
		/// Gets the Contacts resource which allows you to manage your contacts (also sometimes refered to as 'recipients').
		/// </summary>
		/// <value>
		/// The contacts.
		/// </value>
		IContacts Contacts { get; }

		/// <summary>
		/// Gets the CustomFields resource which allows you to manage your custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		ICustomFields CustomFields { get; }

		/// <summary>
		/// Gets the EngagementQuality resource which allows you to retrieve metrics that define the quality of your email program.
		/// </summary>
		/// <value>
		/// The engagement quality resource.
		/// </value>
		IEngagementQuality EngagementQuality { get; }

		/// <summary>
		/// Gets the Lists resource.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		ILists Lists { get; }

		/// <summary>
		/// Gets the Segments resource.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		ISegments Segments { get; }

		/// <summary>
		/// Gets the SenderIdentities resource.
		/// </summary>
		/// <value>
		/// The sender identities.
		/// </value>
		ISenderIdentities SenderIdentities { get; }

		/// <summary>
		/// Gets the SingleSends resource which allows you to manage your single sends (AKA campaigns).
		/// </summary>
		/// <value>
		/// The single sends.
		/// </value>
		ISingleSends SingleSends { get; }
	}
}
