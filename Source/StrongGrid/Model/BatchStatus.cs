using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	public enum BatchStatus
	{
		[Description("pause")]
		Paused,
		[Description("cancel")]
		Canceled
	}
}
