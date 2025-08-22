using Formitable.BetterStack.Logger.Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StrongGrid.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			// Update the StrongGridJsonSerializerContext.cs file (if necessary)
			var serializerContextPath = Path.Combine(Path.GetDirectoryName(GetThisFilePath()), "..\\StrongGrid\\Json\\StrongGridJsonSerializerContext.cs");
			var additionalSerializableTypes = new[]
			{
				typeof(string[]),
				typeof(long[]),
				typeof(bool),
				typeof(StrongGrid.Json.StrongGridJsonObject),
				typeof(StrongGrid.Json.StrongGridJsonObject[]),
			};
			await UpdateJsonSerializerContextAsync("StrongGrid", "StrongGrid.Models", serializerContextPath, additionalSerializableTypes).ConfigureAwait(false);

			var builder = Host.CreateApplicationBuilder();

			ConfigureLogging(builder.Logging);
			ConfigureServices(builder.Services);

			var host = builder.Build();
			await host.StartAsync().ConfigureAwait(false);
		}

		private static async Task UpdateJsonSerializerContextAsync(string projectName, string baseNamespace, string serializerContextPath, Type[] additionalSerializableTypes)
		{
			var tabIndex = 1; // The number of tabs to prepend to each line in the generated file (for indentation purposes)

			var newSerializerContext = new StringBuilder();
			newSerializerContext
				.AppendLine($"// This file is maintained by {projectName}.IntegrationTests.Program.\r\n")
				.AppendLine("using System.Text.Json.Serialization;\r\n")
				.AppendLine("#pragma warning disable CS0618 // Type or member is obsolete")
				.AppendLine($"namespace {projectName}.Json")
				.AppendLine("{");

			var tabs = string.Concat(Enumerable.Repeat("\t", tabIndex));
			foreach (var type in additionalSerializableTypes ?? Enumerable.Empty<Type>())
			{
				newSerializerContext.AppendLine($"{tabs}[JsonSerializable(typeof({type.FullName}))]");
			}

			newSerializerContext
				.AppendLine()
				.AppendLine(GenerateAttributesForSerializerContext(baseNamespace, tabIndex))
				.AppendLine($"\tinternal partial class {projectName}JsonSerializerContext : JsonSerializerContext")
				.AppendLine("\t{")
				.AppendLine("\t}")
				.AppendLine("}")
				.AppendLine("#pragma warning restore CS0618 // Type or member is obsolete");

			var currentSerializerContext = await File.ReadAllTextAsync(serializerContextPath, CancellationToken.None).ConfigureAwait(false);

			if (newSerializerContext.ToString() != currentSerializerContext)
			{
				await File.WriteAllTextAsync(serializerContextPath, newSerializerContext.ToString(), CancellationToken.None).ConfigureAwait(false);

				throw new Exception("The serializer context has been updated. You must restart the integration tests to ensure the recent changes take effect.");
			}
		}

		private static string GenerateAttributesForSerializerContext(string baseNamespace, int tabIndex = 0)
		{
			// Handy code to generate the 'JsonSerializable' attributes for ZoomNetJsonSerializerContext
			var allTypes = Assembly
				.GetAssembly(typeof(Client))
				.GetTypes()
				.Where(t => t.IsClass || t.IsEnum)
				.Where(t => !string.IsNullOrEmpty(t.Namespace))
				.Where(t => t.Namespace.StartsWith(baseNamespace))
				.Where(t => !t.IsGenericType)
				.Where(t => t.GetCustomAttribute<CompilerGeneratedAttribute>() == null);

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

			var typesSortedAlphabetically = typesInBaseNamespace
				.Union(typesInSubNamespace)
				.OrderBy(t => t.Type.FullName);

			var tabs = string.Concat(Enumerable.Repeat("\t", tabIndex));
			var simpleAttributes = string.Join("\r\n", typesSortedAlphabetically.Where(t => !string.IsNullOrEmpty(t.JsonSerializeAttribute)).Select(t => $"{tabs}{t.JsonSerializeAttribute}"));
			var arrayAttributes = string.Join("\r\n", typesSortedAlphabetically.Where(t => !string.IsNullOrEmpty(t.JsonSerializeAttributeArray)).Select(t => $"{tabs}{t.JsonSerializeAttributeArray}"));
			var nullableAttributes = string.Join("\r\n", typesSortedAlphabetically.Where(t => !string.IsNullOrEmpty(t.JsonSerializeAttributeNullable)).Select(t => $"{tabs}{t.JsonSerializeAttributeNullable}"));

			var result = string.Join("\r\n\r\n", [simpleAttributes, arrayAttributes, nullableAttributes]);
			return result;
		}

		// From: https://stackoverflow.com/questions/47841441/how-do-i-get-the-path-to-the-current-c-sharp-source-code-file
		private static string GetThisFilePath([CallerFilePath] string path = null)
		{
			return path;
		}

		private static void ConfigureLogging(ILoggingBuilder logging)
		{
			logging.ClearProviders();

			var betterStackToken = Environment.GetEnvironmentVariable("BETTERSTACK_TOKEN");
			if (!string.IsNullOrEmpty(betterStackToken))
			{
				logging.AddBetterStackLogger(options =>
				{
					options.SourceToken = betterStackToken;
					options.Context["source"] = "StrongGrid_integration_tests";
					options.Context["StrongGrid-Version"] = StrongGrid.Client.Version;
				});
			}

			logging.AddSimpleConsole(options =>
			{
				options.SingleLine = true;
				options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
			});

			logging
				.SetMinimumLevel(LogLevel.Debug) // Set the minimum log level to Debug
				.AddFilter<ConsoleLoggerProvider>(logLevel => logLevel > LogLevel.Information); // Filter out logs below Information level for ConsoleLoggerProvider
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			// -----------------------------------------------------------------------------
			// Do you want to proxy requests through a tool such as Fiddler? Very useful for debugging.
			var useProxy = true;

			// By default Fiddler Classic uses port 8888 and Fiddler Everywhere uses port 8866
			var proxyPort = 8888;

			// Change the default values to avoid being overwhelmed by too many debug messages in the console
			var clientOptions = new StrongGridClientOptions()
			{
				// Trigger a 'Trace' log (rather than the default 'Debug') when a successful call is made.
				// This is to ensure that we don't get overwhelmed by too many debug messages in the console.
				LogLevelSuccessfulCalls = LogLevel.Trace,
				LogLevelFailedCalls = LogLevel.Error,
			};
			// -----------------------------------------------------------------------------

			// Configure StrongGrid client
			var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");

			// Configure the proxy if desired
			var proxy = useProxy ? new WebProxy($"http://localhost:{proxyPort}") : null;

			services.AddStrongGrid(apiKey, proxy, clientOptions);
			services.AddLegacyStrongGrid(apiKey, proxy, clientOptions);

			services.AddHostedService<TestsRunner>();
		}
	}
}
