using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	public class Alert
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("email_to")]
		public string EmailTo { get; set; }

		[JsonProperty("frequency")]
		public Frequency Frequency { get; set; }

		[JsonProperty("type")]
		public AlertType Type { get; set; }

		[JsonProperty("created_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

		[JsonProperty("updated_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }
	}
}
