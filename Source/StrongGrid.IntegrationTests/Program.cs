using Logzio.DotNet.NLog;
using Microsoft.ApplicationInsights.NLogTarget;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Linq;
using System.Text;
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
				logzioTarget.ContextProperties.Add(new NLog.Targets.TargetPropertyWithContext("Source", "StrongGrid_integration_tests"));
				logzioTarget.ContextProperties.Add(new NLog.Targets.TargetPropertyWithContext("StrongGrid-Version", StrongGrid.Client.Version));

				nLogConfig.AddTarget("Logzio", logzioTarget);
				nLogConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logzioTarget, "*");
			}

			// Send logs to Azure Insights
			var instrumentationKey = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_INSTRUMENTATION_KEY");
			if (!string.IsNullOrEmpty(instrumentationKey))
			{
				var applicationInsightsTarget = new ApplicationInsightsTarget() { InstrumentationKey = instrumentationKey, Name = "StrongGrid" };
				applicationInsightsTarget.ContextProperties.Add(new Microsoft.ApplicationInsights.NLogTarget.TargetPropertyWithContext("Source", "StrongGrid_integration_tests"));
				applicationInsightsTarget.ContextProperties.Add(new Microsoft.ApplicationInsights.NLogTarget.TargetPropertyWithContext("StrongGrid-Version", StrongGrid.Client.Version));

				nLogConfig.AddTarget("ApplicationInsights", applicationInsightsTarget);
				nLogConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, applicationInsightsTarget, "*");
			}

			// Send logs to DataDog
			var datadogKey = Environment.GetEnvironmentVariable("DATADOG_APIKEY");
			if (!string.IsNullOrEmpty(datadogKey))
			{
				var datadogTarget = new WebServiceTarget("datadog")
				{
					Url = "https://http-intake.logs.us5.datadoghq.com/v1/input",
					Encoding = Encoding.UTF8,
					Protocol = WebServiceProtocol.JsonPost,
					PreAuthenticate = false
				};

				// DD_API_KEY
				// Your Datadog API Key for sending your logs to Datadog.
				datadogTarget.Headers.Add(new MethodCallParameter("DD-API-KEY", Layout.FromString(datadogKey)));

				// DD_SITE
				// The name of your Datadog site.Choose from one of the following examples:
				// Example: datadoghq.com(US1), datadoghq.eu(EU), us3.datadoghq.com(US3), us5.datadoghq.com(US5), ddog - gov.com(US1 - FED)
				// Default: datadoghq.com(US1)
				datadogTarget.Headers.Add(new MethodCallParameter("DD_SITE", Layout.FromString("us5.datadoghq.com(US5)")));

				// DD_LOGS_DIRECT_SUBMISSION_INTEGRATIONS
				// Enables Agentless logging.Enable for your logging framework by setting to Serilog, NLog, Log4Net, or ILogger(for Microsoft.Extensions.Logging).
				// If you are using multiple logging frameworks, use a semicolon separated list of variables.
				// Example: Serilog; Log4Net; NLog
				datadogTarget.Headers.Add(new MethodCallParameter("DD_LOGS_DIRECT_SUBMISSION_INTEGRATIONS", Layout.FromString("NLog")));

				// DD_LOGS_DIRECT_SUBMISSION_SOURCE
				// Sets the parsing rule for submitted logs.
				// Should always be set to csharp, unless you have a custom pipeline.
				// Default: csharp
				datadogTarget.Headers.Add(new MethodCallParameter("DD_LOGS_DIRECT_SUBMISSION_SOURCE", Layout.FromString("csharp")));

				// DD_LOGS_DIRECT_SUBMISSION_MAX_BATCH_SIZE
				// Sets the maximum number of logs to send at one time.
				// Takes into account the limits in place for the API.
				// Default: 1000

				// DD_LOGS_DIRECT_SUBMISSION_MAX_QUEUE_SIZE
				// Sets the maximum number of logs to hold in the internal queue at any one time before dropping log messages.
				// Default: 100000

				// DD_LOGS_DIRECT_SUBMISSION_BATCH_PERIOD_SECONDS
				// Sets the time to wait(in seconds) before checking for new logs to send.
				// Default: 1

				datadogTarget.Headers.Add(new MethodCallParameter("Content-Type", Layout.FromString("application/json")));
				datadogTarget.Headers.Add(new MethodCallParameter("Source", "StrongGrid_integration_tests"));
				datadogTarget.Headers.Add(new MethodCallParameter("StrongGrid-Version", StrongGrid.Client.Version));

				nLogConfig.AddTarget("DataDog", datadogTarget);
				nLogConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, datadogTarget, "*");
			}

			// Send logs to console
			var consoleTarget = new ColoredConsoleTarget();
			nLogConfig.AddTarget("ColoredConsole", consoleTarget);
			nLogConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, consoleTarget, "*");

			return nLogConfig;
		}
	}
}
