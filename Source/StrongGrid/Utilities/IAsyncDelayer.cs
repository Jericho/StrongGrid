using System;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	public interface IAsyncDelayer
	{
		Task Delay(TimeSpan timeSpan);
	}
}
