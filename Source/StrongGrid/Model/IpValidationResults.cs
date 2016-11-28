using Newtonsoft.Json;

namespace StrongGrid.Model
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
		[JsonProperty("a_record")]
		public ValidationResult ARecord { get; set; }
	}
}
