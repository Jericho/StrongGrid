using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;

namespace StrongGrid.Model
{
	/// <summary>
	/// This object can be thought of as an envelope - it defines who should receive an individual
	/// message and how that message should be handled. For more information, please see our
	/// documentation on Personalizations. Parameters in personalizations will override the parameters
	/// of the same name from the message level.
	/// </summary>
	public class MailPersonalization
	{
		/// <summary>
		/// Gets or sets an array of recipients.
		/// Each <see cref="MailAddress"/> object within this array may contain the recipient’s
		/// name, but must always contain the recipient’s email.
		/// </summary>
		/// <value>
		/// To.
		/// </value>
		[JsonProperty("to", NullValueHandling = NullValueHandling.Ignore)]
		public MailAddress[] To { get; set; }

		/// <summary>
		/// Gets or sets an array of recipients who will receive a copy of your email.
		/// Each <see cref="MailAddress"/> object within this array may contain the recipient’s
		/// name, but must always contain the recipient’s email.
		/// </summary>
		/// <value>
		/// The cc.
		/// </value>
		[JsonProperty("cc", NullValueHandling = NullValueHandling.Ignore)]
		public MailAddress[] Cc { get; set; }

		/// <summary>
		/// Gets or sets an array of recipients who will receive a blind carbon copy of your email.
		/// Each <see cref="MailAddress"/> object within this array may contain the recipient’s
		/// name, but must always contain the recipient’s email.
		/// </summary>
		/// <value>
		/// The BCC.
		/// </value>
		[JsonProperty("bcc", NullValueHandling = NullValueHandling.Ignore)]
		public MailAddress[] Bcc { get; set; }

		/// <summary>
		/// Gets or sets the subject line of your email.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the headers which allow you to specify specific handling instructions for your email.
		/// </summary>
		/// <value>
		/// The headers.
		/// </value>
		[JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(KeyValuePairEnumerationConverter))]
		public KeyValuePair<string, string>[] Headers { get; set; }

		/// <summary>
		/// Gets or sets the substitutions.
		/// </summary>
		/// <value>
		/// The substitutions.
		/// </value>
		[JsonProperty("substitutions", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(KeyValuePairEnumerationConverter))]
		public KeyValuePair<string, string>[] Substitutions { get; set; }

		/// <summary>
		/// Gets or sets the custom arguments.
		/// </summary>
		/// <value>
		/// The custom arguments.
		/// </value>
		[JsonProperty("custom_args", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(KeyValuePairEnumerationConverter))]
		public KeyValuePair<string, string>[] CustomArguments { get; set; }

		/// <summary>
		/// Gets or sets the timestamp allowing you to specify when you want your email to be sent from SendGrid.
		/// </summary>
		/// <value>
		/// The send at.
		/// </value>
		[JsonProperty("send_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? SendAt { get; set; }
	}
}
