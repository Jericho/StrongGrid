using Logzio.DotNet.NLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
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

			var builder = Host.CreateApplicationBuilder();
			builder.Services.AddHostedService<TestsRunner>();

			// Configure cancellation (this allows you to press CTRL+C or CTRL+Break to stop the integration tests)
			var cts = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) =>
			{
				e.Cancel = true;
				cts.Cancel();
			};

			// Configure logging
			builder.Logging.ClearProviders(); // Remove the built-in providers (which include the Console)
			builder.Logging.AddNLog(GetNLogConfiguration()); // Add our desired custom providers (which include the Colored Console)

			// Run the tests
			var host = builder.Build();
			await host.StartAsync(cts.Token).ConfigureAwait(false);

			// Stop NLog (which has the desirable side-effect of flushing any pending logs)
			LogManager.Shutdown();
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

		private static LoggingConfiguration GetNLogConfiguration()
		{
			// Configure logging
			var nLogConfig = new LoggingConfiguration();

			// Send logs to logz.io
			var logzioToken = Environment.GetEnvironmentVariable("LOGZIO_TOKEN");
			if (!string.IsNullOrEmpty(logzioToken))
			{
				var logzioTarget = new LogzioTarget
				{
					Name = "Logzio",
					Token = logzioToken,
					LogzioType = "nlog",
					JsonKeysCamelCase = true,
					// ProxyAddress = "http://localhost:8888",
				};
				logzioTarget.ContextProperties.Add(new TargetPropertyWithContext("Source", "StrongGrid_integration_tests"));
				logzioTarget.ContextProperties.Add(new TargetPropertyWithContext("StrongGrid-Version", StrongGrid.Client.Version));

				nLogConfig.AddTarget("Logzio", logzioTarget);
				nLogConfig.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logzioTarget, "*");
			}

			// Send logs to console
			var consoleTarget = new ColoredConsoleTarget();
			nLogConfig.AddTarget("ColoredConsole", consoleTarget);
			nLogConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, consoleTarget, "*");

			return nLogConfig;
		}
	}
}
