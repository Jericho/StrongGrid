using Formitable.BetterStack.Logger.Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var serializerContext = GenerateAttributesForSerializerContext();

			// Configure cancellation (this allows you to press CTRL+C or CTRL+Break to stop the integration tests)
			var cts = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) =>
			{
				e.Cancel = true;
				cts.Cancel();
			};

			var services = new ServiceCollection();
			ConfigureServices(services);
			using var serviceProvider = services.BuildServiceProvider();
			var app = serviceProvider.GetService<IHostedService>();
			await app.StartAsync(cts.Token).ConfigureAwait(false);
		}

		private static void ConfigureServices(ServiceCollection services)
		{
			services.AddHostedService<TestsRunner>();

			services
				.AddLogging(logging =>
				{
					var betterStackToken = Environment.GetEnvironmentVariable("BETTERSTACK_TOKEN");
					if (!string.IsNullOrEmpty(betterStackToken))
					{
						logging.AddBetterStackLogger(options =>
						{
							options.SourceToken = betterStackToken;
							options.Context["source"] = "ZoomNet_integration_tests";
							options.Context["StrongGrid-Version"] = StrongGrid.Client.Version;
						});
					}

					logging.AddSimpleConsole(options =>
					{
						options.SingleLine = true;
						options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
					});

					logging.AddFilter(logLevel => logLevel >= LogLevel.Debug);
					logging.AddFilter<ConsoleLoggerProvider>(logLevel => logLevel >= LogLevel.Information);
				});
		}

		private static string GenerateAttributesForSerializerContext()
		{
			// Handy code to generate the 'JsonSerializable' attributes for ZoomNetJsonSerializerContext
			var baseNamespace = "StrongGrid.Models";
			var allTypes = System.Reflection.Assembly
				.GetAssembly(typeof(Client))
				.GetTypes()
				.Where(t => t.IsClass || t.IsEnum)
				.Where(t => !string.IsNullOrEmpty(t.Namespace))
				.Where(t => t.Namespace.StartsWith(baseNamespace));

			var typesInBaseNamespace = allTypes
				.Where(t => t.Namespace.Equals(baseNamespace))
				.Select(t => new
				{
					Type = t,
					JsonSerializeAttribute = $"[JsonSerializable(typeof({t.FullName}))]",
					JsonSerializeAttributeArray = $"[JsonSerializable(typeof({t.FullName}[]))]",
					JsonSerializeAttributeNullable = t.IsEnum ? $"[JsonSerializable(typeof({t.FullName}?))]" : string.Empty,
				});

			var typesInSubNamespace = allTypes
				.Where(t => !t.Namespace.Equals(baseNamespace))
				.Select(t => new
				{
					Type = t,
					JsonSerializeAttribute = $"[JsonSerializable(typeof({t.FullName}), TypeInfoPropertyName = \"{t.FullName.Remove(0, baseNamespace.Length + 1).Replace(".", "")}\")]",
					JsonSerializeAttributeArray = $"[JsonSerializable(typeof({t.FullName}[]), TypeInfoPropertyName = \"{t.FullName.Remove(0, baseNamespace.Length + 1).Replace(".", "")}Array\")]",
					JsonSerializeAttributeNullable = t.IsEnum ? $"[JsonSerializable(typeof({t.FullName}?), TypeInfoPropertyName = \"{t.FullName.Remove(0, baseNamespace.Length + 1).Replace(".", "")}Nullable\")]" : string.Empty,
				});

			var typesSortedAlphabetically = typesInBaseNamespace.Union(typesInSubNamespace).OrderBy(t => t.Type.FullName);

			var simpleAttributes = string.Join("\r\n", typesSortedAlphabetically.Where(t => !string.IsNullOrEmpty(t.JsonSerializeAttribute)).Select(t => t.JsonSerializeAttribute));
			var arrayAttributes = string.Join("\r\n", typesSortedAlphabetically.Where(t => !string.IsNullOrEmpty(t.JsonSerializeAttributeArray)).Select(t => t.JsonSerializeAttributeArray));
			var nullableAttributes = string.Join("\r\n", typesSortedAlphabetically.Where(t => !string.IsNullOrEmpty(t.JsonSerializeAttributeNullable)).Select(t => t.JsonSerializeAttributeNullable));

			var result = string.Join("\r\n\r\n", [simpleAttributes, arrayAttributes, nullableAttributes]);
			return result;
		}
	}
}
