using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Unsubscribe options.
	/// </summary>
	public class UnsubscribeOptions
	{
		/// <summary>
		/// Gets or sets the group identifier.
		/// </summary>
		/// <value>
		/// The group identifier.
		/// </value>
		[JsonPropertyName("group_id")]
		public long GroupId { get; set; }

		/// <summary>
		/// Gets or sets the groups to display.
		/// </summary>
		/// <value>
		/// The groups to display.
		/// </value>
		[JsonPropertyName("groups_to_display")]
		public int[] GroupsToDisplay { get; set; }
	}
}
