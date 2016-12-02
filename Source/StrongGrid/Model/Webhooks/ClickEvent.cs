using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model.Webhooks
{
	/// <summary>
	/// Recipient clicked on a link within the message.
	/// You need to enable Click Tracking for getting this type of event.
	/// </summary>
	/// <seealso cref="StrongGrid.Model.Webhooks.EngagementEvent" />
	public class ClickEvent : EngagementEvent
	{
		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the URL offset.
		/// </summary>
		/// <value>
		/// The URL offset.
		/// </value>
		[JsonProperty("url_offset", NullValueHandling = NullValueHandling.Ignore)]
		public UrlOffset UrlOffset { get; set; }

		/// <summary>
		/// Gets or sets the asm group identifier.
		/// </summary>
		/// <value>
		/// The asm group identifier.
		/// </value>
		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public int AsmGroupId { get; set; }

		/// <summary>
		/// Gets or sets the newsletter.
		/// </summary>
		/// <value>
		/// The newsletter.
		/// </value>
		[JsonProperty("newsletter", NullValueHandling = NullValueHandling.Ignore)]
		public Newsletter Newsletter { get; set; }
	}
}
