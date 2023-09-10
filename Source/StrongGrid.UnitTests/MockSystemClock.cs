using StrongGrid.Utilities;
using System;

namespace StrongGrid.UnitTests
{
	internal class MockSystemClock : ISystemClock
	{
		private readonly DateTime _now;

		public DateTime Now => _now;

		public DateTime UtcNow => _now;

		public MockSystemClock(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			_now = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
		}
	}
}
