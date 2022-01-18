using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// This object can be thought of as an envelope - it defines who should receive an individual
	/// message and how that message should be handled. For more information, please see our
	/// documentation on Personalizations. Parameters in personalizations will override the parameters
	/// of the same name from the message level.
	/// </summary>
	[JsonConverter(typeof(MailPersonalizationConverter))]
	public class MailPersonalization
	{
		/// <summary>
		/// Gets or sets the 'From' email address used to deliver the message.
		/// This address should be a verified sender in your Twilio SendGrid account.
		/// </summary>
		/// <value>
		/// From.
		/// </value>
		[JsonPropertyName("from")]
		public MailAddress From { get; set; }

		/// <summary>
		/// Gets or sets an array of recipients.
		/// Each <see cref="MailAddress"/> object within this array may contain the recipient’s
		/// name, but must always contain the recipient’s email.
		/// </summary>
		/// <value>
		/// To.
		/// </value>
		[JsonPropertyName("to")]
		public MailAddress[] To { get; set; }

		/// <summary>
		/// Gets or sets an array of recipients who will receive a copy of your email.
		/// Each <see cref="MailAddress"/> object within this array may contain the recipient’s
		/// name, but must always contain the recipient’s email.
		/// </summary>
		/// <value>
		/// The cc.
		/// </value>
		[JsonPropertyName("cc")]
		public MailAddress[] Cc { get; set; }

		/// <summary>
		/// Gets or sets an array of recipients who will receive a blind carbon copy of your email.
		/// Each <see cref="MailAddress"/> object within this array may contain the recipient’s
		/// name, but must always contain the recipient’s email.
		/// </summary>
		/// <value>
		/// The BCC.
		/// </value>
		[JsonPropertyName("bcc")]
		public MailAddress[] Bcc { get; set; }

		/// <summary>
		/// Gets or sets the subject line of your email.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonPropertyName("subject")]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the headers which allow you to specify specific handling instructions for your email.
		/// </summary>
		/// <value>
		/// The headers.
		/// </value>
		[JsonPropertyName("headers")]
		[JsonConverter(typeof(KeyValuePairEnumerationConverter))]
		public KeyValuePair<string, string>[] Headers { get; set; }

		/// <summary>
		/// Gets or sets the substitutions.
		/// </summary>
		/// <value>
		/// The substitutions.
		/// </value>
		/// <remarks>
		/// This is used when sending emails without a template or when using a 'legacy' template.
		/// Substitutions are ignored when using a dynamic template.
		/// For dynamic templates, you must use <see cref="DynamicData"/>.
		/// </remarks>
		[JsonPropertyName("substitutions")]
		[JsonConverter(typeof(KeyValuePairEnumerationConverter))]
		public KeyValuePair<string, string>[] Substitutions { get; set; }

		/// <summary>
		/// Gets or sets the merge data.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		/// <remarks>
		/// This is used when sending emails with a dynamic template.
		/// DynamicData is ignored when using a legacy template.
		/// For legacy templates, you must use <see cref="Substitutions"/>.
		/// </remarks>
		[JsonPropertyName("dynamic_template_data")]
		public object DynamicData { get; set; }

		/// <summary>
		/// Gets or sets the custom arguments.
		/// </summary>
		/// <value>
		/// The custom arguments.
		/// </value>
		[JsonPropertyName("custom_args")]
		[JsonConverter(typeof(KeyValuePairEnumerationConverter))]
		public KeyValuePair<string, string>[] CustomArguments { get; set; }

		/// <summary>
		/// Gets or sets the timestamp allowing you to specify when you want your email to be sent from SendGrid.
		/// </summary>
		/// <value>
		/// The send at.
		/// </value>
		[JsonPropertyName("send_at")]
		[JsonConverter(typeof(NullableEpochConverter))]
		public DateTime? SendAt { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a dynamic template is use to personalize the content sent to the recipients.
		/// This is for internal use only. It is used by the <see cref="MailPersonalizationConverter"/> to generate the appropriate JSON.
		/// </summary>
		internal bool IsUsedWithDynamicTemplate { get; set; }
	}
}
