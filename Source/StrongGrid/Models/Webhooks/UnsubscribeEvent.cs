namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Recipient clicked on the ‘Opt Out of All Emails' link (available after clicking the message's subscription management link).
	/// You need to enable Subscription Tracking for getting this type of event.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.EngagementEvent" />
	public class UnsubscribeEvent : EngagementEvent
	{
	}
}
