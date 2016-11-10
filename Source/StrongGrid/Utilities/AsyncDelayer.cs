using System;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	public class AsyncDelayer : IAsyncDelayer
	{
		public Task Delay(TimeSpan timeSpan)
		{
			return Task.Delay(timeSpan);
		}
	}
}
