using System;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Interface for the SystemClock.
	/// </summary>
	internal interface ISystemClock
	{
		/// <summary>
		/// Gets a <see cref="DateTime" /> object that is set to the current date and time on this
		/// computer, expressed as the local time.
		/// </summary>
		/// <value>
		/// The current date and time, expressed as the local time.
		/// </value>
		DateTime Now { get; }

		/// <summary>
		/// Gets a System.DateTime object that is set to the current date and time on this
		/// computer, expressed as the Coordinated Universal Time (UTC).
		/// </summary>
		/// <value>
		/// The current date and time, expressed as the Coordinated Universal Time (UTC).
		/// </value>
		DateTime UtcNow { get; }
	}
}
