using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace StrongGrid.Benchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			//var baseNamespace = "StrongGrid.Models";
			//var allTypes = Assembly
			//	.GetAssembly(typeof(BaseClient))
			//	.GetTypes()
			//	.Where(t => t.IsClass)
			//	.Where(t => !string.IsNullOrEmpty(t.Namespace))
			//	.Where(t => t.Namespace.StartsWith(baseNamespace))
			//	.Where(t => !t.Namespace.StartsWith(baseNamespace + ".Webhooks.InboundEmail"));	// Exclude inbound email classes which are deserialized by our WebhookParser

			//var typesInBaseNamespace = allTypes
			//	.Where(t => t.Namespace.Equals(baseNamespace))
			//	.Select(t => new { Type = t, JsonSerializeAttribute = $"[JsonSerializable(typeof({t.FullName}))]" });

			//var typesInSubNamespace = allTypes
			//	.Where(t => !t.Namespace.Equals(baseNamespace))
			//	.Select(t => new { Type = t, JsonSerializeAttribute = $"[JsonSerializable(typeof({t.FullName}), TypeInfoPropertyName = \"{t.FullName.Remove(0, baseNamespace.Length + 1).Replace(".", "")}\")]" });

			//var result = string.Join("\r\n", typesInBaseNamespace.Union(typesInSubNamespace).OrderBy(t => t.Type.FullName).Select(t => t.JsonSerializeAttribute));

			IConfig config = null;

			// To debug
			//config = new DebugInProcessConfig();

			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
		}
	}
}
