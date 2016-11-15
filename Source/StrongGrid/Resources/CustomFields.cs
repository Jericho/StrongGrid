using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	public class CustomFields
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the Recipients class
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint, do not prepend slash</param>
		public CustomFields(IClient client, string endpoint = "/contactdb/custom_fields")
		{
			_endpoint = endpoint;
			_client = client;
		}

		public async Task<CustomFieldMetadata> CreateAsync(string name, FieldType type, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name },
				{ "type", JToken.Parse(JsonConvert.SerializeObject(type, Formatting.None, new StringEnumConverter())).Value<string>() }
			};
			var response = await _client.PostAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var field = JObject.Parse(responseContent).ToObject<CustomFieldMetadata>();
			return field;
		}

		public async Task<CustomFieldMetadata[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			//{
			//  "custom_fields": [
			//    {
			//      "id": 1,
			//      "name": "birthday",
			//      "type": "date"
			//    },
			//    {
			//      "id": 2,
			//      "name": "middle_name",
			//      "type": "text"
			//    },
			//    {
			//      "id": 3,
			//      "name": "favorite_number",
			//      "type": "number"
			//    }
			//  ]
			//}
			// We use a dynamic object to get rid of the 'custom_fields' property and simply return an array of custom fields
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.custom_fields;

			var fields = dynamicArray.ToObject<CustomFieldMetadata[]>();
			return fields;
		}

		public async Task<CustomFieldMetadata> GetAsync(int fieldId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}", _endpoint, fieldId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var field = JObject.Parse(responseContent).ToObject<CustomFieldMetadata>();
			return field;
		}

		public async Task DeleteAsync(int fieldId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}", _endpoint, fieldId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		public async Task<Field[]> GetReservedFieldsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/contactdb/reserved_fields", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			//{
			//  "reserved_fields": [
			//    {
			//      "name": "first_name",
			//      "type": "text"
			//    },
			//    {
			//      "name": "last_name",
			//      "type": "text"
			//    },
			//    {
			//      "name": "email",
			//      "type": "text"
			//    }
			//  ]
			//}
			// We use a dynamic object to get rid of the 'reserved_fields' property and simply return an array of fields
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.reserved_fields;

			var fields = dynamicArray.ToObject<Field[]>();
			return fields;
		}
	}
}
