using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Unsubscribe options
	/// </summary>
	public class UnsubscribeOptions
	{
		/// <summary>
		/// Gets or sets the group identifier.
		/// </summary>
		/// <value>
		/// The group identifier.
		/// </value>
		[JsonProperty("group_id", NullValueHandling = NullValueHandling.Ignore)]
		public long GroupId { get; set; }

		/// <summary>
		/// Gets or sets the groups to display.
		/// </summary>
		/// <value>
		/// The groups to display.
		/// </value>
		[JsonProperty("groups_to_display", NullValueHandling = NullValueHandling.Ignore)]
		public int[] GroupsToDisplay { get; set; }
	}
}
