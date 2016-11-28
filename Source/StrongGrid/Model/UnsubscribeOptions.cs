using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class UnsubscribeOptions
	{
		/// <summary>
		/// Gets or sets the group identifier.
		/// </summary>
		/// <value>
		/// The group identifier.
		/// </value>
		[JsonProperty("group_id")]
		public int GroupId { get; set; }

		/// <summary>
		/// Gets or sets the groups to display.
		/// </summary>
		/// <value>
		/// The groups to display.
		/// </value>
		[JsonProperty("groups_to_display")]
		public int[] GroupsToDisplay { get; set; }
	}
}
