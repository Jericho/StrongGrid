using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	public class SenderIdentity
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the nick.
		/// </summary>
		/// <value>
		/// The name of the nick.
		/// </value>
		[JsonProperty("nickname")]
		public string NickName { get; set; }

		/// <summary>
		/// Gets or sets from.
		/// </summary>
		/// <value>
		/// From.
		/// </value>
		[JsonProperty("from")]
		public MailAddress From { get; set; }

		/// <summary>
		/// Gets or sets the reply to.
		/// </summary>
		/// <value>
		/// The reply to.
		/// </value>
		[JsonProperty("reply_to")]
		public MailAddress ReplyTo { get; set; }

		/// <summary>
		/// Gets or sets the address1.
		/// </summary>
		/// <value>
		/// The address1.
		/// </value>
		[JsonProperty("address")]
		public string Address1 { get; set; }

		/// <summary>
		/// Gets or sets the address2.
		/// </summary>
		/// <value>
		/// The address2.
		/// </value>
		[JsonProperty("address_2")]
		public string Address2 { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>
		/// The city.
		/// </value>
		[JsonProperty("city")]
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
		[JsonProperty("state")]
		public string State { get; set; }

		/// <summary>
		/// Gets or sets the zip.
		/// </summary>
		/// <value>
		/// The zip.
		/// </value>
		[JsonProperty("zip")]
		public string Zip { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		/// <value>
		/// The country.
		/// </value>
		[JsonProperty("country")]
		public string Country { get; set; }

		/// <summary>
		/// Gets or sets the verification.
		/// </summary>
		/// <value>
		/// The verification.
		/// </value>
		[JsonProperty("verified")]
		public VerificationStatus Verification { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SenderIdentity"/> is locked.
		/// </summary>
		/// <value>
		///   <c>true</c> if locked; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("locked")]
		public bool Locked { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonProperty("created_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonProperty("updated_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }
	}
}
