using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Linq;

namespace StrongGrid.Benchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			//var serializerContext = GenerateAttributesForSerializerContext();

			IConfig config = null;

			// To debug
			//config = new DebugInProcessConfig();

			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
		}

		private static string GenerateAttributesForSerializerContext()
		{
			// Handy code to generate the 'JsonSerializable' attributes for StrongGridJsonSerializerContext
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

			var result = string.Join("\r\n\r\n", new[] { simpleAttributes, arrayAttributes, nullableAttributes });
			return result;
		}
	}
}
