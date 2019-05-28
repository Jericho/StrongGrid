using System;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Logging behavior.
	/// Allows you to decide if only successful calls, only failed calls, both or neither should be logged.
	/// </summary>
	[Flags]
	public enum LogBehavior : short
	{
		/// <summary>
		/// Do not log any calls.
		/// </summary>
		LogNothing = 0,

		/// <summary>
		/// Log successful calls (i.e.: calls with a response StatusCode in the 200-299 range).
		/// </summary>
		LogSuccessfulCalls = 1 << 0,

		/// <summary>
		/// Log failed calls.
		/// </summary>
		LogFailedCalls = 1 << 1,

		/// <summary>
		/// Log all calls.
		/// </summary>
		LogEverything = LogSuccessfulCalls | LogFailedCalls
	}
}
