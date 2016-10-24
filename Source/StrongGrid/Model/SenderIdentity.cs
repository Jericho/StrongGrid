using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	public class SenderIdentity
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("nickname")]
		public string NickName { get; set; }

		[JsonProperty("from")]
		public MailAddress From { get; set; }

		[JsonProperty("reply_to")]
		public MailAddress ReplyTo { get; set; }

		[JsonProperty("address")]
		public string Address1 { get; set; }

		[JsonProperty("address2")]
		public string Address2 { get; set; }

		[JsonProperty("city")]
		public string City { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("zip")]
		public string Zip { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("verified")]
		public VerificationStatus Verification { get; set; }

		[JsonProperty("locked")]
		public bool Locked { get; set; }

		[JsonProperty("created_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

		[JsonProperty("updated_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }
	}
}
