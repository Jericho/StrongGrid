using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Setting that are used by the <see cref="WarmupEngine"/>.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class WarmupSettings
	{
		private static int[] _sendGridRecommendedDailyVolume = new[]
		{
			50,
			100,
			500,
			1000,
			5000,
			10000,
			20000,
			40000,
			70000,
			100000,
			150000,
			250000,
			400000,
			600000,
			1000000,
			2000000,
			4000000,
			8000000,
			16000000,
			32000000,
			64000000,
			128000000,
			256000000,
			512000000
		};

		/// <summary>
		/// Gets or sets the name of the IP pool.
		/// </summary>
		public string PoolName { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of emails that can be sent each day during the warmup process.
		/// </summary>
		public int[] DailyVolumePerIpAddress { get; set; }

		/// <summary>
		/// Gets or sets the number of days allowed between today and lastSendDate, default 1.
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
		/// <param name="resetDays">The number of days allowed between today and lastSendDate, default 1. For instance, if you send emails on a daily basis, this value should be 1. If you send on a weekly basis, this number should be 7.</param>
		public WarmupSettings(string poolName, int[] dailyVolumePerIpAddress, int resetDays = 1)
		{
			PoolName = poolName;
			DailyVolumePerIpAddress = dailyVolumePerIpAddress;
			ResetDays = resetDays;
		}

		/// <summary>
		/// Returns "best practice" warmup settings as recommended by SendGrid tailored to your estimated daily send volume.
		/// </summary>
		/// <remarks>
		/// The recommended daily volume values are documented on SendGrid's web site: https://sendgrid.com/docs/assets/IPWarmupSchedule.pdf.
		/// </remarks>
		/// <param name="poolName">The name of the pool.</param>
		/// <param name="estimatedDailyVolume">The number of emails you expect to send in a typical day.</param>
		/// <param name="resetDays">The number of days allowed between today and lastSendDate, default 1. For instance, if you send emails on a daily basis, this value should be 1. If you send on a weekly basis, this number should be 7.</param>
		/// <returns>The warmup settings.</returns>
		public static WarmupSettings FromSendGridRecomendedSettings(string poolName, int estimatedDailyVolume, int resetDays = 1)
		{
			return new WarmupSettings()
			{
				DailyVolumePerIpAddress = _sendGridRecommendedDailyVolume.Where(v => v <= Math.Max(estimatedDailyVolume, _sendGridRecommendedDailyVolume[0])).ToArray(),
				PoolName = poolName,
				ResetDays = resetDays
			};
		}
	}
}
