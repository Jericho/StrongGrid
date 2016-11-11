using System;

namespace StrongGrid.Utilities
{
	public interface ISystemClock
	{
		DateTime Now { get; }
		DateTime UtcNow { get; }
	}
}
