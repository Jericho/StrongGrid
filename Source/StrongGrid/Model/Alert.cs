using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
		[JsonConverter(typeof(StringEnumConverter))]
		public Frequency Frequency { get; set; }

		[JsonProperty("type")]
		[JsonConverter(typeof(StringEnumConverter))]
		public AlertType Type { get; set; }

		[JsonProperty("created_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }

		[JsonProperty("updated_at")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ModifiedOn { get; set; }
	}
}
