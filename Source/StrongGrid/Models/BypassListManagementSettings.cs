using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Allows you to bypass all unsubscribe groups and suppressions to ensure that the email is
	/// delivered to every single recipient. This should only be used in emergencies when it is
	/// absolutely necessary that every recipient receives your email. Ex: outage emails, or forgot
	/// password emails.
	/// </summary>
	public class BypassListManagementSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BypassListManagementSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }
	}
}
