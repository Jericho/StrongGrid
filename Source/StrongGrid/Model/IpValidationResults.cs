using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class IpValidationResults
	{
		/// <summary>
		/// Gets or sets a record.
		/// </summary>
		/// <value>
		/// a record.
		/// </value>
		[JsonProperty("a_record")]
		public ValidationResult ARecord { get; set; }
	}
}
