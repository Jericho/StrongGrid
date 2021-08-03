using System.Text.Json.Serialization;

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
		[JsonPropertyName("a_record")]
		public ValidationResult ARecord { get; set; }
	}
}
