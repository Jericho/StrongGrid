namespace StrongGrid.Warmup
{
	/// <summary>
	/// Information about the result of the email send.
	/// </summary>
	[StrongGrid.Utilities.ExcludeFromCodeCoverage]
	public class WarmupResult
	{
		/// <summary>
		/// Gets a value indicating whether the warmup process is completed or not.
		/// </summary>
		public bool Completed { get; internal set; }

		/// <summary>
		/// Gets the MessageId of the email sent from an IP address in the pool being warmed up.
		/// </summary>
		public string MessageIdOnPool { get; private set; }

		/// <summary>
		/// Gets the MessageId of the email sent from an IP address NOT in the pool being warmed up.
		/// </summary>
		public string MessageIdNotOnPool { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WarmupResult" /> class.
		/// </summary>
		/// <param name="completed">Indicator whether the warmup process is completed or not</param>
		/// <param name="messageIdNotOnPool">The MessageId of the email sent from an IP address in the pool being warmed up</param>
		/// <param name="messageIdOnPool">The MessageId of the email sent from an IP address NOT in the pool being warmed up</param>
		public WarmupResult(bool completed, string messageIdOnPool, string messageIdNotOnPool)
		{
			Completed = completed;
			MessageIdOnPool = messageIdOnPool;
			MessageIdNotOnPool = messageIdNotOnPool;
		}
	}
}
