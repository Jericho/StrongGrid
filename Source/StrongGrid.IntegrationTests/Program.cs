using Formitable.BetterStack.Logger.Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.IO;
using System.Linq;
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
	}
}
