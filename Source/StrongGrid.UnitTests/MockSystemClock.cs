using Moq;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.UnitTests
{
	public class MockSystemClock : Mock<ISystemClock>
	{
		public MockSystemClock(DateTime currentDateTime) :
			this(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second, currentDateTime.Millisecond)
		{ }

		public MockSystemClock(int year, int month, int day, int hour, int minute, int second, int millisecond) :
			base(MockBehavior.Strict)
		{
			SetupGet(m => m.Now).Returns(new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc));
			SetupGet(m => m.UtcNow).Returns(new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc));
		}
	}
}
