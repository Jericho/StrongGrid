using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class ValidationResult
	{
		/// <summary>
		/// Gets or sets a value indicating if valiudation is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("reason")]
		public string Reason { get; set; }
	}
}
