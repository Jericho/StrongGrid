using Newtonsoft.Json.Linq;
using StrongGrid.Resources;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid
{
	public interface IClient
	{
		Mail Mail { get; }
		ApiKeys ApiKeys { get; }
		UnsubscribeGroups UnsubscribeGroups { get; }
		User User { get; }

		Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken cancellationToken = default(CancellationToken));
		Task<HttpResponseMessage> PostAsync(string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken));
		Task<HttpResponseMessage> PostAsync(string endpoint, JArray data, CancellationToken cancellationToken = default(CancellationToken));
		Task<HttpResponseMessage> DeleteAsync(string endpoint, CancellationToken cancellationToken = default(CancellationToken));
		Task<HttpResponseMessage> DeleteAsync(string endpoint, JObject data = null, CancellationToken cancellationToken = default(CancellationToken));
		Task<HttpResponseMessage> DeleteAsync(string endpoint, JArray data = null, CancellationToken cancellationToken = default(CancellationToken));
		Task<HttpResponseMessage> PatchAsync(string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken));
		Task<HttpResponseMessage> PatchAsync(string endpoint, JArray data, CancellationToken cancellationToken = default(CancellationToken));
		Task<HttpResponseMessage> PutAsync(string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken));
	}
}
