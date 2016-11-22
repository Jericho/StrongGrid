using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class BypassListManagementSettings
	{
		[JsonProperty("enable")]
		public bool Enabled { get; set; }
	}
}
