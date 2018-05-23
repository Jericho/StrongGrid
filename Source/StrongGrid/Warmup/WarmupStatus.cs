using Newtonsoft.Json;
using System;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Information about the status of the warmup process.
	/// </summary>
	[StrongGrid.Utilities.ExcludeFromCodeCoverage]
	public class WarmupStatus
	{
		/// <summary>
		/// Gets or sets the name of the IP Pool
		/// </summary>
		[JsonProperty("pool_name", NullValueHandling = NullValueHandling.Ignore)]
		public string PoolName { get; set; }

		/// <summary>
		/// Gets or sets the IP addresses to warmup
		/// </summary>
		[JsonProperty("ip_addresses", NullValueHandling = NullValueHandling.Ignore)]
		public string[] IpAddresses { get; set; }

		/// <summary>
		/// Gets or sets the warmup day.
		/// 1 represents the first day of the process, 2 represents the second day, and so forth.
		/// Zero indicates that the process hasn't started yet.
		/// </summary>
		[JsonProperty("warmup_day", NullValueHandling = NullValueHandling.Ignore)]
		public int WarmupDay { get; set; }

		/// <summary>
		/// Gets or sets the last day emails were sent through the IP pool
		/// </summary>
		[JsonProperty("date_last_sent", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime DateLastSent { get; set; }

		/// <summary>
		/// Gets or sets the number of emails that have been sent during the last day
		/// </summary>
		[JsonProperty("emails_sent_last_day", NullValueHandling = NullValueHandling.Ignore)]
		public int EmailsSentLastDay { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the warmup process is completed or not.
		/// </summary>
		[JsonProperty("completed", NullValueHandling = NullValueHandling.Ignore)]
		public bool Completed { get; set; }
	}
}
