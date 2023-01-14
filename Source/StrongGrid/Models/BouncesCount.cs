using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// The count of bounces for a given classification.
	/// </summary>
	public class BouncesCount
	{
		/// <summary>Gets or sets the bounce classification.</summary>
		[JsonPropertyName("classification")]
		public BounceClassification Classification { get; set; }

		/// <summary>Gets or sets the domain.</summary>
		[JsonPropertyName("domain")]
		public string Domain { get; set; }

		/// <summary>Gets or sets the count.</summary>
		[JsonPropertyName("count")]
		public long Count { get; set; }
	}
}
