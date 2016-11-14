using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class SegmentsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/contactdb/segments";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_SEGMENT_JSON = @"{
			'id': 1,
			'name': 'Last Name Miller',
			'list_id': 4,
			'conditions': [
				{
					'field': 'last_name',
					'value': 'Miller',
					'operator': 'eq',
					'and_or': ''
				},
				{
					'field': 'last_clicked',
					'value': '01/02/2015',
					'operator': 'gt',
					'and_or': 'and'
				},
				{
					'field': 'clicks.campaign_identifier',
					'value': '513',
					'operator': 'eq',
					'and_or': 'or'
				}
			]
		}";
		private const string MULTIPLE_SEGMENTS_JSON = @"{
			'segments': [
				{
					'id': 1,
					'name': 'Last Name Miller',
					'list_id': 4,
					'conditions': [
						{
							'field': 'last_name',
							'value': 'Miller',
							'operator': 'eq',
							'and_or': ''
						}
					],
					'recipient_count': 1
				}
			]
		}";

		#endregion

		private Segments CreateSegments()
		{
			return new Segments(_mockClient.Object, ENDPOINT);

		}

		[TestInitialize]
		public void TestInitialize()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_mockClient = _mockRepository.Create<IClient>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_mockRepository.VerifyAll();
		}

		[TestMethod]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Segment>(SINGLE_SEGMENT_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Conditions);
			Assert.AreEqual(3, result.Conditions.Length);

			Assert.AreEqual("last_name", result.Conditions[0].Field);
			Assert.AreEqual(LogicalOperator.None, result.Conditions[0].LogicalOperator);
			Assert.AreEqual(ConditionOperator.Equal, result.Conditions[0].Operator);
			Assert.AreEqual("Miller", result.Conditions[0].Value);

			Assert.AreEqual("last_clicked", result.Conditions[1].Field);
			Assert.AreEqual(LogicalOperator.And, result.Conditions[1].LogicalOperator);
			Assert.AreEqual(ConditionOperator.GreatherThan, result.Conditions[1].Operator);
			Assert.AreEqual("01/02/2015", result.Conditions[1].Value);

			Assert.AreEqual("clicks.campaign_identifier", result.Conditions[2].Field);
			Assert.AreEqual(LogicalOperator.Or, result.Conditions[2].LogicalOperator);
			Assert.AreEqual(ConditionOperator.Equal, result.Conditions[2].Operator);
			Assert.AreEqual("513", result.Conditions[2].Value);

			Assert.AreEqual(1, result.Id);
			Assert.AreEqual(4, result.ListId);
			Assert.AreEqual("Last Name Miller", result.Name);
		}

		[TestMethod]
		public void Create()
		{
			// Arrange
			var name = "Last Name Miller";
			var listId = 4;
			var conditions = new[]
			{
				new SearchCondition
				{
					Field = "last_name",
					Value= "Miller",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.None
				},
				new SearchCondition
				{
					Field = "last_clicked",
					Value = "01/02/2015",
					Operator = ConditionOperator.GreatherThan,
					LogicalOperator = LogicalOperator.And
				},
				new SearchCondition
				{
					Field = "clicks.campaign_identifier",
					Value = "513",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.Or
				}
			};

			_mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SEGMENT_JSON) })
				.Verifiable();

			var segments = CreateSegments();

			// Act
			var result = segments.CreateAsync(name, listId, conditions, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_SEGMENTS_JSON) })
				.Verifiable();

			var segments = CreateSegments();

			// Act
			var result = segments.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var segmentId = 1;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{segmentId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SEGMENT_JSON) })
				.Verifiable();

			var segments = CreateSegments();

			// Act
			var result = segments.GetAsync(segmentId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Update()
		{
			// Arrange
			var segmentId = 5;
			var name = "The Millers";
			var listId = 5;
			var conditions = new[]
			{
				new SearchCondition
				{
					Field = "last_name",
					Value= "Miller",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.None
				}
			};

			_mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{segmentId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SEGMENT_JSON) })
				.Verifiable();

			var segments = CreateSegments();

			// Act
			var result = segments.UpdateAsync(segmentId, name, listId, conditions, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Delete_and_preserve_contacts()
		{
			// Arrange
			var segmentId = 1;
			var deleteContacts = false;

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{segmentId}?delete_contacts=false", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var segments = CreateSegments();

			// Act
			segments.DeleteAsync(segmentId, deleteContacts, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Delete_and_delete_contacts()
		{
			// Arrange
			var segmentId = 1;
			var deleteContacts = true;

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{segmentId}?delete_contacts=true", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var segments = CreateSegments();

			// Act
			segments.DeleteAsync(segmentId, deleteContacts, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void GetRecipients()
		{
			// Arrange
			var segmentId = 1;
			var recordsPerPage = 25;
			var page = 1;

			var apiResponse = @"{
				'recipients': [
					{
						'created_at': 1422313607,
						'email': 'jones@example.com',
						'first_name': null,
						'id': 'YUBh',
						'last_clicked': null,
						'last_emailed': null,
						'last_name': 'Jones',
						'last_opened': null,
						'updated_at': 1422313790,
						'custom_fields': [
							{
								'id': 23,
								'name': 'pet',
								'value': 'Indiana',
								'type': 'text'
							}
						]
					}
				]
			}";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{segmentId}/recipients?page_size={recordsPerPage}&page={page}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var segments = CreateSegments();

			// Act
			var result = segments.GetRecipientsAsync(segmentId, recordsPerPage, page, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("jones@example.com", result[0].Email);
		}
	}
}
