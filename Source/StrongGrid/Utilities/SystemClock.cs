using System;

namespace StrongGrid.Utilities
{
	public class SystemClock : ISystemClock
	{
		#region FIELDS

		private static readonly Lazy<ISystemClock> _instance = new Lazy<ISystemClock>(() => new SystemClock(), true);

		#endregion

		#region PROPERTIES

		public static ISystemClock Instance { get { return _instance.Value; } }
		public DateTime Now { get { return DateTime.Now; } }
		public DateTime UtcNow { get { return DateTime.UtcNow; } }

		#endregion

		#region CONSTRUCTOR

		private SystemClock() { }

		#endregion
	}
}
