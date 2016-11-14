using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	public enum BatchStatus
	{
		[EnumMember(Value = "pause")]
		Paused,
		[EnumMember(Value = "cancel")]
		Canceled
	}
}
