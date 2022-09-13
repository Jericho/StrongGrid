using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Segment status.
	/// </summary>
	public class QueryValidationStatus
	{
		/// <summary>
		/// Gets or sets the object that indicates whether the segment's contacts will be updated periodically.
		/// </summary>
		[JsonPropertyName("query_validation")]
		public QueryValidationStatusType Status { get; set; }

		/// <summary>
		/// Gets or sets the mesage that describes any errors that were encountered during query validation.
		/// </summary>
		[JsonPropertyName("error_message")]
		public string ErrorMessage { get; set; }
	}
}
