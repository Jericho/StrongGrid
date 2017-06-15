using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// IP validation results
	/// </summary>
	public class IpValidationResults
	{
		/// <summary>
		/// Gets or sets a record.
		/// </summary>
		/// <value>
		/// a record.
		/// </value>
		[JsonProperty("a_record", NullValueHandling = NullValueHandling.Ignore)]
		public ValidationResult ARecord { get; set; }
	}
}
