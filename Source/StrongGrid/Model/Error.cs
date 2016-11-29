using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// An error
	/// </summary>
	public class Error
	{
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		[JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the error indices.
		/// </summary>
		/// <value>
		/// The error indices.
		/// </value>
		[JsonProperty("error_indices", NullValueHandling = NullValueHandling.Ignore)]
		public int[] ErrorIndices { get; set; }
	}
}
