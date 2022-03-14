using StrongGrid.Json;
using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Alerts allow you to specify an email address to receive notifications regarding your email usage or statistics.
	/// - Usage alerts allow you to set the threshold at which an alert will be sent.For example, if you want to be
	/// notified when you've used 90% of your current package's allotted emails, you would set the "percentage"
	/// parameter to 90.
	/// - Stats notifications allow you to set how frequently you would like to receive email
	/// statistics reports.For example, if you want to receive your stats notifications every day, simply set the
	/// "frequency" parameter to "daily". Stats notifications include data such as how many emails you sent each day,
	/// in addition to other email events such as bounces, drops, unsubscribes, etc.
	/// </summary>
	public class Alert
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the email to.
		/// </summary>
		/// <value>
		/// The email to.
		/// </value>
		[JsonPropertyName("email_to")]
		public string EmailTo { get; set; }

		/// <summary>
		/// Gets or sets the frequency.
		/// </summary>
		/// <value>
		/// The frequency.
		/// </value>
		[JsonPropertyName("frequency")]
		public Frequency Frequency { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonPropertyName("type")]
		public AlertType Type { get; set; }

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
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonPropertyName("updated_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }
	}
}
