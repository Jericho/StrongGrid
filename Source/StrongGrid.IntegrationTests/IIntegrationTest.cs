using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests
{
	public interface IIntegrationTest
	{
		Task RunAsync(Client client, TextWriter log, CancellationToken cancellationToken);
	}
}
