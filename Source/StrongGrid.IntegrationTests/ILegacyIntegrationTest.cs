using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests
{
	public interface ILegacyIntegrationTest
	{
		Task RunAsync(LegacyClient client, TextWriter log, CancellationToken cancellationToken);
	}
}
