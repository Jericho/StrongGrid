﻿using Newtonsoft.Json.Linq;
using StrongGrid.Resources;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid
{
	public interface IClient
	{
		ApiKeys ApiKeys { get; }
		Categories Categories { get; }
		Contacts Contacts { get; }
		CustomFields CustomFields { get; }
		GlobalSuppressions GlobalSuppressions { get; }
		Lists Lists { get; }
		Mail Mail { get; }
		Segments Segments { get; }
		Statistics Statistics { get; }
		Suppressions Suppressions { get; }
		Templates Templates { get; }
		UnsubscribeGroups UnsubscribeGroups { get; }
		User User { get; }
		string Version { get; }

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
