using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StrongGrid.Model
{
	public class MailPersonalization
	{
		/// <summary>
		/// Gets or sets to.
		/// </summary>
		/// <value>
		/// To.
		/// </value>
		[JsonProperty("to")]
		public MailAddress[] To { get; set; }

		/// <summary>
		/// Gets or sets the cc.
		/// </summary>
		/// <value>
		/// The cc.
		/// </value>
		[JsonProperty("cc")]
		public MailAddress[] CC { get; set; }

		/// <summary>
		/// Gets or sets the BCC.
		/// </summary>
		/// <value>
		/// The BCC.
		/// </value>
		[JsonProperty("bcc")]
		public MailAddress[] BCC { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonProperty("subject")]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the headers.
		/// </summary>
		/// <value>
		/// The headers.
		/// </value>
		[JsonProperty("headers")]
		public KeyValuePair<string, string>[] Headers { get; set; }

		/// <summary>
		/// Gets or sets the send at.
		/// </summary>
		/// <value>
		/// The send at.
		/// </value>
		[JsonProperty("sendat")]
		public DateTime? SendAt { get; set; }
	}
}
