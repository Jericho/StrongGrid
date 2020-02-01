using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests
{
	public interface IIntegrationTest
	{
		Task RunAsync(IBaseClient baseClient, TextWriter log, CancellationToken cancellationToken);
	}
}
