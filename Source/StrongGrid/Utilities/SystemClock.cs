using System;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// A replacement for .Net <see cref="DateTime.Now" /> and <see cref="DateTime.UtcNow" />.
	/// </summary>
	/// <seealso cref="StrongGrid.Utilities.ISystemClock" />
	public class SystemClock : ISystemClock
	{
		#region FIELDS

		private static readonly Lazy<ISystemClock> _instance = new Lazy<ISystemClock>(() => new SystemClock(), true);

		#endregion

		#region PROPERTIES

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static ISystemClock Instance { get { return _instance.Value; } }

		/// <summary>
		/// Gets a <see cref="DateTime" /> object that is set to the current date and time on this
		/// computer, expressed as the local time.
		/// </summary>
		/// <value>
		/// The current date and time, expressed as the local time.
		/// </value>
		public DateTime Now { get { return DateTime.Now; } }

		/// <summary>
		/// Gets a System.DateTime object that is set to the current date and time on this
		/// computer, expressed as the Coordinated Universal Time (UTC).
		/// </summary>
		/// <value>
		/// The current date and time, expressed as the Coordinated Universal Time (UTC).
		/// </value>
		public DateTime UtcNow { get { return DateTime.UtcNow; } }

		#endregion

		#region CONSTRUCTOR

		private SystemClock() { }

		#endregion
	}
}
