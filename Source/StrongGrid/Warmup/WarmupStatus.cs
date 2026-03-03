using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Information about the status of the warmup process.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class WarmupStatus
	{
		/// <summary>
		/// Gets or sets the name of the IP Pool.
		/// </summary>
		[JsonPropertyName("pool_name")]
		public string PoolName { get; set; }

		/// <summary>
		/// Gets or sets the IP addresses to warmup.
		/// </summary>
		[JsonPropertyName("ip_addresses")]
		public string[] IpAddresses { get; set; }

		/// <summary>
		/// Gets or sets the warmup day.
		/// 1 represents the first day of the process, 2 represents the second day, and so forth.
		/// Zero indicates that the process hasn't started yet.
		/// </summary>
		[JsonPropertyName("warmup_day")]
		public int WarmupDay { get; set; }

		/// <summary>
		/// Gets or sets the last day emails were sent through the IP pool.
		/// </summary>
		[JsonPropertyName("date_last_sent")]
		public DateTime DateLastSent { get; set; }

		/// <summary>
		/// Gets or sets the number of emails that have been sent during the last day.
		/// </summary>
		[JsonPropertyName("emails_sent_last_day")]
		public int EmailsSentLastDay { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the warmup process is completed or not.
		/// </summary>
		[JsonPropertyName("completed")]
		public bool Completed { get; set; }
	}
}
