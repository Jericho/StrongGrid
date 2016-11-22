using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SandboxModeSettings
	{
		[JsonProperty("enable")]
		public bool Enabled { get; set; }
	}
}
