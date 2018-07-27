using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Reverse DNS validation results.
	/// </summary>
	public class ReverseDnsValidationResults
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
