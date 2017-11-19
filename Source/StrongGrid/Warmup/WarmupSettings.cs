namespace StrongGrid.Warmup
{
	/// <summary>
	/// Setting that are used by the <see cref="WarmupEngine"/>.
	/// </summary>
	public class WarmupSettings
	{
		/// <summary>
		/// Gets or sets the name of the IP pool.
		/// </summary>
		public string PoolName { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of emails that can be sent each day during the warmup process.
		/// </summary>
		public int[] DailyVolumePerIpAddress { get; set; }

		/// <summary>
		/// Gets or sets the number of days allowed between today() and lastSendDate, default 1.
		/// </summary>
		public int ResetDays { get; set; } = 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="WarmupSettings" /> class.
		/// </summary>
		public WarmupSettings()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WarmupSettings" /> class.
		/// </summary>
		/// <param name="poolName">The name of the pool.</param>
		/// <param name="dailyVolumePerIpAddress">The maximum number of emails that can be sent each day during the warmup process.</param>
		/// <param name="resetDays">The number of days allowed between today() and lastSendDate, Default 1. For instance, if you send emails on a daily basis, this value should be 1. If you send on a weekly basis, this number should be 7.</param>
		public WarmupSettings(string poolName, int[] dailyVolumePerIpAddress, int resetDays = 1)
		{
			PoolName = poolName;
			DailyVolumePerIpAddress = dailyVolumePerIpAddress;
			ResetDays = resetDays;
		}
	}
}
