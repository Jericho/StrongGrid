using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// An error.
	/// </summary>
	public class Error
	{
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		[JsonPropertyName("message")]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the error indices.
		/// </summary>
		/// <value>
		/// The error indices.
		/// </value>
		[JsonPropertyName("error_indices")]
		public int[] ErrorIndices { get; set; }
	}
}
