using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	public class BatchInfo
	{
		[JsonProperty("batch_id")]
		public string BatchId { get; set; }

		[JsonProperty("status")]
		[JsonConverter(typeof(StringEnumConverter))]
		public BatchStatus Status { get; set; }
	}
}
