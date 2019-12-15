using StrongGrid.Models;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Templates : IIntegrationTest
	{
		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** TEMPLATES *****\n").ConfigureAwait(false);

			// GET TEMPLATES
			var legacyTemplates = await client.Templates.GetAllAsync(TemplateType.Legacy, null, cancellationToken).ConfigureAwait(false);
			var dynamicTemplates = await client.Templates.GetAllAsync(TemplateType.Dynamic, null, cancellationToken).ConfigureAwait(false);
			var templates = legacyTemplates.Union(dynamicTemplates);
			await log.WriteLineAsync($"All templates retrieved. There are {legacyTemplates.Length} legacy templates and {dynamicTemplates.Length} dynamic templates").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = templates
				.Where(t => t.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldTemplate =>
				{
					foreach (var oldTemplateVersion in oldTemplate.Versions)
					{
						await client.Templates.DeleteVersionAsync(oldTemplateVersion.TemplateId, oldTemplateVersion.Id, null, cancellationToken).ConfigureAwait(false);
						await log.WriteLineAsync($"Template version {oldTemplateVersion.TemplateId}.{oldTemplateVersion.Id} deleted").ConfigureAwait(false);
					}
					await client.Templates.DeleteAsync(oldTemplate.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Template {oldTemplate.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// Legacy
			var legacyTemplate = await client.Templates.CreateAsync("StrongGrid Integration Testing: My legacy template", TemplateType.Legacy, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy template '{legacyTemplate.Name}' created. Id: {legacyTemplate.Id}");

			legacyTemplate = await client.Templates.UpdateAsync(legacyTemplate.Id, "StrongGrid Integration Testing: Legacy template updated name", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy template '{legacyTemplate.Id}' updated").ConfigureAwait(false);

			legacyTemplate = await client.Templates.GetAsync(legacyTemplate.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Template '{legacyTemplate.Id}' retrieved.").ConfigureAwait(false);

			var firstLegacyVersion = await client.Templates.CreateVersionAsync(legacyTemplate.Id, "StrongGrid Integration Testing: Legacy version 1", "My first Subject <%subject%>", "<html<body>hello world<br/><%body%></body></html>", "Hello world <%body%>", true, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"First legacy version created. Id: {firstLegacyVersion.Id}").ConfigureAwait(false);

			var secondLegacyVersion = await client.Templates.CreateVersionAsync(legacyTemplate.Id, "StrongGrid Integration Testing: Legacy version 2", "My second Subject <%subject%>", "<html<body>Qwerty<br/><%body%></body></html>", "Qwerty <%body%>", true, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Second legacy version created. Id: {secondLegacyVersion.Id}").ConfigureAwait(false);

			await client.Templates.DeleteVersionAsync(legacyTemplate.Id, firstLegacyVersion.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy version {firstLegacyVersion.Id} deleted").ConfigureAwait(false);

			await client.Templates.DeleteVersionAsync(legacyTemplate.Id, secondLegacyVersion.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy version {secondLegacyVersion.Id} deleted").ConfigureAwait(false);

			await client.Templates.DeleteAsync(legacyTemplate.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy template {legacyTemplate.Id} deleted").ConfigureAwait(false);

			// Dynamic
			var dynamicTemplate = await client.Templates.CreateAsync("StrongGrid Integration Testing: My dynamic template", TemplateType.Dynamic, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic template '{dynamicTemplate.Name}' created. Id: {dynamicTemplate.Id}");

			dynamicTemplate = await client.Templates.UpdateAsync(dynamicTemplate.Id, "StrongGrid Integration Testing: Dynamic template updated name", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic template '{dynamicTemplate.Id}' updated").ConfigureAwait(false);

			dynamicTemplate = await client.Templates.GetAsync(dynamicTemplate.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Template '{dynamicTemplate.Id}' retrieved.").ConfigureAwait(false);

			var firstDynamicVersion = await client.Templates.CreateVersionAsync(dynamicTemplate.Id, "StrongGrid Integration Testing: Dynamic version 1", "Dear {{first_name}}", "<html<body>hello world<br/></body></html>", "Hello world", true, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"First dynamic version created. Id: {firstDynamicVersion.Id}").ConfigureAwait(false);

			var dynamicHtmlContent = @"
<html>
	<body>
		Hello {{Customer.first_name}} {{Customer.last_name}}. 
		You have a credit balance of {{CreditBalance}}<br/>
		<ol>
		{{#each Orders}}
			<li>You ordered: {{this.item}} on: {{this.date}}</li>
		{{/each}}
		</ol>
	</body>
</html>";
			var testData = new
			{
				Customer = new
				{
					first_name = "aaa",
					last_name = "aaa"
				},
				CreditBalance = 99.88,
				Orders = new[]
				{
					new { item = "item1", date = "1/1/2018" },
					new { item = "item2", date = "1/2/2018" },
					new { item = "item3", date = "1/3/2018" }
				}
			};

			var secondDynamicVersion = await client.Templates.CreateVersionAsync(dynamicTemplate.Id, "StrongGrid Integration Testing: Dynamic version 2", "Dear {{Customer.first_name}}", dynamicHtmlContent, "... this is the text content ...", true, EditorType.Code, testData, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Second dynamic version created. Id: {secondDynamicVersion.Id}").ConfigureAwait(false);

			await client.Templates.DeleteVersionAsync(dynamicTemplate.Id, firstDynamicVersion.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic version {firstDynamicVersion.Id} deleted").ConfigureAwait(false);

			await client.Templates.DeleteVersionAsync(dynamicTemplate.Id, secondDynamicVersion.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic version {secondDynamicVersion.Id} deleted").ConfigureAwait(false);

			await client.Templates.DeleteAsync(dynamicTemplate.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic template {dynamicTemplate.Id} deleted").ConfigureAwait(false);
		}
	}
}
