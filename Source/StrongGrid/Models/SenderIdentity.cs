using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Models
{
	/// <summary>
	/// Sender identity.
	/// </summary>
	public class SenderIdentity
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the nick.
		/// </summary>
		/// <value>
		/// The name of the nick.
		/// </value>
		[JsonProperty("nickname", NullValueHandling = NullValueHandling.Ignore)]
		public string NickName { get; set; }

		/// <summary>
		/// Gets or sets from.
		/// </summary>
		/// <value>
		/// From.
		/// </value>
		[JsonIgnore]
		public MailAddress From
		{
			get => new MailAddress(this.FromEmail, this.FromName);

			set
			{
				this.FromEmail = value?.Email;
				this.FromName = value?.Name;
			}
		}

		/// <summary>
		/// Gets or sets the reply to.
		/// </summary>
		/// <value>
		/// The reply to.
		/// </value>
		[JsonIgnore]
		public MailAddress ReplyTo
		{
			get => new MailAddress(this.ReplyToEmail, this.ReplyToName);

			set
			{
				this.ReplyToEmail = value?.Email;
				this.ReplyToName = value?.Name;
			}
		}

		/// <summary>
		/// Gets or sets the address1.
		/// </summary>
		/// <value>
		/// The address1.
		/// </value>
		[JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
		public string Address1 { get; set; }

		/// <summary>
		/// Gets or sets the address2.
		/// </summary>
		/// <value>
		/// The address2.
		/// </value>
		[JsonProperty("address_2", NullValueHandling = NullValueHandling.Ignore)]
		public string Address2 { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>
		/// The city.
		/// </value>
		[JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
		[JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
		public string State { get; set; }

		/// <summary>
		/// Gets or sets the zip.
		/// </summary>
		/// <value>
		/// The zip.
		/// </value>
		[JsonProperty("zip", NullValueHandling = NullValueHandling.Ignore)]
		public string Zip { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		/// <value>
		/// The country.
		/// </value>
		[JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
		public string Country { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SenderIdentity"/> is verified.
		/// </summary>
		/// <value>
		/// The verification.
		/// </value>
		[JsonProperty("verified", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsVerified { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SenderIdentity"/> is locked.
		/// </summary>
		/// <value>
		///   <c>true</c> if locked; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("locked", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsLocked { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }

		[JsonProperty("from_email", NullValueHandling = NullValueHandling.Ignore)]
		private string FromEmail { get; set; }

		[JsonProperty("from_name", NullValueHandling = NullValueHandling.Ignore)]
		private string FromName { get; set; }

		[JsonProperty("reply_to", NullValueHandling = NullValueHandling.Ignore)]
		private string ReplyToEmail { get; set; }

		[JsonProperty("reply_to_name", NullValueHandling = NullValueHandling.Ignore)]
		private string ReplyToName { get; set; }
	}
}
