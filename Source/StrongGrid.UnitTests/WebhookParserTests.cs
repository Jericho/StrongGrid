using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Models.Webhooks;
using StrongGrid.Utilities;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class WebhookParserTests
	{
		#region FIELDS

		private const string PROCESSED_JSON = @"
		{
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'event':'processed',
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1,
			'send_at':1249949000
		}";

		private const string BOUNCED_JSON = @"
		{
			'status':'5.0.0',
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'event':'bounce',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1,
			'reason':'500 No Such User',
			'type':'bounce',
			'ip' : '127.0.0.1',
			'tls' : '1',
			'cert_err' : '0'
		}";

		private const string DEFERRED_JSON = @"
		{
			'response':'400 Try again',
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'event':'deferred',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'attempt':'10',
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1,
			'ip' : '127.0.0.1',
			'tls' : '0',
			'cert_err' : '0'
		}";

		private const string DROPPED_JSON = @"
		{
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'reason':'Bounced Address',
			'event':'dropped'
		}";

		private const string DELIVERED_JSON = @"
		{
			'response':'250 OK',
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'event':'delivered',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1,
			'ip' : '127.0.0.1',
			'tls' : '1',
			'cert_err' : '1'
		}";

		private const string CLICK_JSON = @"
		{
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'ip':'255.255.255.255',
			'useragent':'Mozilla/5.0 (iPhone; CPU iPhone OS 7_1_2 like Mac OS X) AppleWebKit/537.51.2 (KHTML, like Gecko) Version/7.0 Mobile/11D257 Safari/9537.53',
			'event':'click',
			'email':'email@example.com',
			'timestamp':1249948800,
			'url':'http://yourdomain.com/blog/news.html',
			'url_offset': {
				'index': 0,
				'type': 'html'
			},
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1
		}";

		private const string OPEN_JSON = @"
		{
			'email':'email@example.com',
			'timestamp':1249948800,
			'ip':'255.255.255.255',
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'useragent':'Mozilla/5.0 (Windows NT 5.1; rv:11.0) Gecko Firefox/11.0 (via ggpht.com GoogleImageProxy)',
			'event':'open',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1
		}";

		private const string SPAMREPORT_JSON = @"
		{
			'smtp-id':'<original-smtp-id@domain.com>',
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'email':'email@example.com',
			'timestamp':1249948800,
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'event':'spamreport',
			'asm_group_id': 1
		}";

		private const string UNSUBSCRIBE_JSON = @"
		{
			'smtp-id':'<original-smtp-id@domain.com>',
			'sg_message_id':'sendgrid_internal_message_id',
			'email':'email@example.com',
			'timestamp':1249948800,
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'event':'unsubscribe',
			'asm_group_id': 1
		}";

		private const string GROUPUNSUBSCRIBE_JSON = @"
		{
			'smtp-id':'<original-smtp-id@domain.com>',
			'sg_message_id':'sendgrid_internal_message_id',
			'email':'email@example.com',
			'timestamp':1249948800,
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'event':'group_unsubscribe',
			'asm_group_id':1,
			'useragent':'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36',
			'ip':'255.255.255.255'
		}";

		private const string GROUPRESUBSCRIBE_JSON = @"
		{
			'smtp-id':'<original-smtp-id@domain.com>',
			'sg_message_id':'sendgrid_internal_message_id',
			'email':'email@example.com',
			'timestamp':1249948800,
			'asm_group_id': 1,
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'event':'group_resubscribe',
			'asm_group_id':1,
			'useragent':'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36',
			'ip':'255.255.255.255'
		}";

		private string INBOUND_EMAIL_WEBHOOK = @"--xYzZY
Content-Disposition: form-data; name=""dkim""

{@hotmail.com : pass}
--xYzZY
Content-Disposition: form-data; name=""envelope""

{""to"":[""test@api.yourdomain.com""],""from"":""bob@example.com""}
--xYzZY
Content-Disposition: form-data; name=""subject""

Test #1
--xYzZY
Content-Disposition: form-data; name=""charsets""

{""to"":""UTF-8"",""html"":""us-ascii"",""subject"":""UTF-8"",""from"":""UTF-8"",""text"":""us-ascii""}
--xYzZY
Content-Disposition: form-data; name=""SPF""

softfail
--xYzZY
Content-Disposition: form-data; name=""headers""

Received: by mx0036p1las1.sendgrid.net with SMTP id JtK4a8OKW4 Wed, 25 Oct 2017 22:36:45 +0000 (UTC)
Received: from NAM03-BY2-obe.outbound.protection.outlook.com (unknown[10.43.24.23]) by mx0036p1las1.sendgrid.net (Postfix) with ESMTPS id 210E420432A for <test @api.yourdomain.com>; Wed, 25 Oct 2017 22:36:44 +0000 (UTC)
DKIM-Signature: v=1; a=rsa-sha256; c=relaxed/relaxed; d=hotmail.com; s=selector1; h=From:Date:Subject:Message-ID:Content-Type:MIME-Version; bh=v+swJ1aNYbEcg9bJpD94LKJL7bPGzXmyfchznPUUm3o=; b=Xd5Nx/eKt5gGYAhtLt4cPR4V+3lIbuaCTK+NeDBE61haLrmOS3h66woY27Rofk6bqpoVzlhq8qqtX3wp7cGaslDSYbOMKXJ0T7mn56/BVhIcVyNuz0PSNbTEKAQHoJzwVbp3b4VU/H3ZYgNVlYoSgDtyC3n52u2GtAYokEbvYRs1v501tCsf5MDZCVav9XYOb7TsvOHDca6SjX4n7rokHIovaPEp86gy2xxAz+EisUngYzJ3WFH1yLTNcsTvHJL+S/IBDR73ZgWX2PLo9lh0SdR/F5/wLhkaOFlyCD7oSRPBOBJgfZKtmZGh5P2e/hI0X1THRM4Fl3rRLWwrdGUA4Q==
Received: from BY2NAM03FT010.eop-NAM03.prod.protection.outlook.com (10.152.84.60) by BY2NAM03HT109.eop-NAM03.prod.protection.outlook.com (10.152.85.95) with Microsoft SMTP Server(version= TLS1_2, cipher= TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384_P384) id 15.20.178.5; Wed, 25 Oct 2017 22:36:43 +0000
Received: from BY2PR04MB1989.namprd04.prod.outlook.com(10.152.84.59) by BY2NAM03FT010.mail.protection.outlook.com(10.152.84.122) with Microsoft SMTP Server(version= TLS1_2, cipher= TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256_P256) id 15.20.178.5 via Frontend Transport; Wed, 25 Oct 2017 22:36:43 +0000
Received: from BY2PR04MB1989.namprd04.prod.outlook.com([10.166.111.17]) by BY2PR04MB1989.namprd04.prod.outlook.com([10.166.111.17]) with mapi id 15.20.0156.007; Wed, 25 Oct 2017 22:36:43 +0000
From: Bob Smith<bob@example.com>
To: ""Test Recipient"" <test @api.yourdomain.com>
Subject: Test #1
Thread-Topic: Test #1
Thread-Index: AdNN4bXcGTp4NdbjRHGNZNWefHx/Gg==
Date: Wed, 25 Oct 2017 22:36:43 +0000
Message-ID: <BY2PR04MB19890F956D5521DA3B25F85BCD440 @BY2PR04MB1989.namprd04.prod.outlook.com>
Accept-Language: en-US
Content-Language: en-US
X-MS-Has-Attach:
X-MS-TNEF-Correlator:
x-incomingtopheadermarker: OriginalChecksum:6E601097DDEC6FC57E546B55598497FC1691DF179D48A390E1D00BF5D97DCC85; UpperCasedChecksum:B7D711ACA2768A91C1F2659CAA244146EA1FF640E75765587AEE1BF8F9E27CE4;SizeAsReceived:6758;Count:43
x-tmn: [fhxQvwKmgzB/h6K9GZxYNsdaZLsNk3xU]
x-ms-publictraffictype: Email
x-microsoft-exchange-diagnostics: 1;BY2NAM03HT109;7:bk3VKGh39PrHhS2gX1krsQuu0T2egofcgz52wEPlumRalehclvcU2NX5JzDMUszjK8+hlQovvIJ/KfP1R+INiCo1Sn+FtXBForMk0IDaECwdb9Z4ceCxF/D0eOnAMvEHSXbK17Tcyy0RpNyBuVCuNyNO9YU0IxU+8ff8Le8CnP/TwM+ae22nkJy74bMcBYGFm5x2J7w/JRCKR9m9+CZ8nor9RFfWL7WPHjBKCfGzexck6IYLGJ3+T6AD5x8cfe1+
x-incomingheadercount: 43
x-eopattributedmessage: 0
x-ms-office365-filtering-correlation-id: 7806427c-f04d-469f-e178-08d51bf8de1f
x-microsoft-antispam: UriScan:;BCL:0;PCL:0;RULEID:(22001)(201702061074)(5061506573)(5061507331)(1603103135)(2017031320274)(2017031324274)(2017031323274)(2017031322404)(1603101448)(1601125374)(1701031045);SRVR:BY2NAM03HT109;
x-ms-traffictypediagnostic: BY2NAM03HT109:
x-exchange-antispam-report-test: UriScan:(21748063052155);
x-exchange-antispam-report-cfa-test: BCL:0;PCL:0;RULEID:(100000700101)(100105000095)(100000701101)(100105300095)(100000702101)(100105100095)(444000031);SRVR:BY2NAM03HT109;BCL:0;PCL:0;RULEID:(100000800101)(100110000095)(100000801101)(100110300095)(100000802101)(100110100095)(100000803101)(100110400095)(100000804101)(100110200095)(100000805101)(100110500095);SRVR:BY2NAM03HT109;
x-forefront-prvs: 0471B73328
x-forefront-antispam-report: SFV:NSPM;SFS:(7070007)(98901004);DIR:OUT;SFP:1901;SCL:1;SRVR:BY2NAM03HT109;H:BY2PR04MB1989.namprd04.prod.outlook.com;FPR:;SPF:None;LANG:;
spamdiagnosticoutput: 1:99
spamdiagnosticmetadata: NSPM
Content-Type: multipart/alternative; boundary=""_000_BY2PR04MB19890F956D5521DA3B25F85BCD440BY2PR04MB1989namp_""
MIME-Version: 1.0
X-OriginatorOrg: hotmail.com
X-MS-Exchange-CrossTenant-Network-Message-Id: 7806427c-f04d-469f-e178-08d51bf8de1f
X-MS-Exchange-CrossTenant-originalarrivaltime: 25 Oct 2017 22:36:43.4326 (UTC)
X-MS-Exchange-CrossTenant-fromentityheader: Internet
X-MS-Exchange-CrossTenant-id: 84df9e7f-e9f6-40af-b435-aaaaaaaaaaaa
X-MS-Exchange-Transport-CrossTenantHeadersStamped: BY2NAM03HT109

--xYzZY
Content-Disposition: form-data; name=""to""

""Test Recipient"" <test@api.yourdomain.com>
--xYzZY
Content-Disposition: form-data; name=""html""

<html xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:w=""urn:schemas-microsoft-com:office:word"" xmlns:m=""http://schemas.microsoft.com/office/2004/12/omml"" xmlns=""http://www.w3.org/TR/REC-html40"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=us-ascii"">
<meta name = ""Generator"" content=""Microsoft Word 15 (filtered medium)"">
<style><!--
/* Font Definitions */
@font-face
	{font-family:""Cambria Math"";
	panose-1:2 4 5 3 5 4 6 3 2 4;}
@font-face
	{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;}
/* Style Definitions */
p.MsoNormal, li.MsoNormal, div.MsoNormal
	{margin:0in;
	margin-bottom:.0001pt;
	font-size:11.0pt;
	font-family:""Calibri"",sans-serif;}
a:link, span.MsoHyperlink
	{mso-style-priority:99;
	color:#0563C1;
	text-decoration:underline;}
a:visited, span.MsoHyperlinkFollowed
	{mso-style-priority:99;
	color:#954F72;
	text-decoration:underline;}
span.EmailStyle17
	{mso-style-type:personal-compose;
	font-family:""Calibri"",sans-serif;
	color:windowtext;}
.MsoChpDefault
	{mso-style-type:export-only;
	font-family:""Calibri"",sans-serif;}
@page WordSection1
{
	size:8.5in 11.0in;
	margin:1.0in 1.0in 1.0in 1.0in;
}
div.WordSection1
	{page:WordSection1;}
--></style><!--[if gte mso 9]><xml>
<o:shapedefaults v:ext=""edit"" spidmax=""1026"" />
</xml><![endif]--><!--[if gte mso 9]><xml>
<o:shapelayout v:ext=""edit"">
<o:idmap v:ext=""edit"" data=""1"" />
</o:shapelayout></xml><![endif]-->
</head>
<body lang = ""EN-US"" link=""#0563C1"" vlink=""#954F72"">
<div class=""WordSection1"">
<p class=""MsoNormal"">Test #1<o:p></o:p></p>
</div>
</body>
</html>

--xYzZY
Content-Disposition: form-data; name=""from""

Bob Smith<bob@example.com>
--xYzZY
Content-Disposition: form-data; name=""text""

Test #1

--xYzZY
Content-Disposition: form-data; name=""sender_ip""

10.43.24.23
--xYzZY
Content-Disposition: form-data; name=""attachments""

0
--xYzZY--";

		#endregion

		[Fact]
		public void InboundEmail()
		{
			// Arrange
			var parser = new WebhookParser();
			using (var stream = GetStream(INBOUND_EMAIL_WEBHOOK))
			{
				// Act
				var inboundEmail = parser.ParseInboundEmailWebhook(stream);

				// Assert
				inboundEmail.Attachments.ShouldNotBeNull();
				inboundEmail.Attachments.Length.ShouldBe(0);
				inboundEmail.Cc.ShouldNotBeNull();
				inboundEmail.Cc.Length.ShouldBe(0);
				inboundEmail.Charsets.ShouldNotBeNull();
				inboundEmail.Charsets.Length.ShouldBe(5);
				inboundEmail.Dkim.ShouldBe("{@hotmail.com : pass}");
				inboundEmail.From.ShouldNotBeNull();
				inboundEmail.From.Email.ShouldBe("bob@example.com");
				inboundEmail.From.Name.ShouldBe("Bob Smith");
				inboundEmail.Headers.ShouldNotBeNull();
				inboundEmail.Headers.Length.ShouldBe(40);
				inboundEmail.Html.ShouldStartWith("<html", Case.Insensitive);
				inboundEmail.SenderIp.ShouldBe("10.43.24.23");
				inboundEmail.SpamReport.ShouldBeNull();
				inboundEmail.SpamScore.ShouldBeNull();
				inboundEmail.Spf.ShouldBe("softfail");
				inboundEmail.Subject.ShouldBe("Test #1");
				inboundEmail.Text.ShouldBe("Test #1\r\n");
				inboundEmail.To.ShouldNotBeNull();
				inboundEmail.To.Length.ShouldBe(1);
				inboundEmail.To[0].Email.ShouldBe("test@api.yourdomain.com");
				inboundEmail.To[0].Name.ShouldBe("Test Recipient");
			}
		}

		[Fact]
		public void Parse_processed_JSON()
		{
			// Arrange

			// Act
			var result = (ProcessedEvent)JsonConvert.DeserializeObject<Event>(PROCESSED_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeFalse();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Processed);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBeNull();
			result.Newsletter.ShouldNotBeNull();
			result.Newsletter.Id.ShouldBe("1943530");
			result.Newsletter.SendId.ShouldBe("2308608");
			result.Newsletter.UserListId.ShouldBe("10557865");
			result.ProcessedOn.ToUnixTime().ShouldBe(1249949000);
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeFalse();
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_bounced_JSON()
		{
			// Arrange

			// Act
			var result = (BouncedEvent)JsonConvert.DeserializeObject<Event>(BOUNCED_JSON, new WebHookEventConverter());

			// Assert
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeFalse();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Bounce);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("127.0.0.1");
			result.Reason.ShouldBe("500 No Such User");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Status.ShouldBe("5.0.0");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeTrue();
			result.Type.ShouldBe("bounce");
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_deferred_JSON()
		{
			// Arrange

			// Act
			var result = (DeferredEvent)JsonConvert.DeserializeObject<Event>(DEFERRED_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Attempt.ShouldBe(10);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeFalse();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Deferred);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("127.0.0.1");
			result.Newsletter.ShouldNotBeNull();
			result.Newsletter.Id.ShouldBe("1943530");
			result.Newsletter.SendId.ShouldBe("2308608");
			result.Newsletter.UserListId.ShouldBe("10557865");
			result.Response.ShouldBe("400 Try again");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeFalse();
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_dropped_JSON()
		{
			// Arrange

			// Act
			var result = (DroppedEvent)JsonConvert.DeserializeObject<Event>(DROPPED_JSON, new WebHookEventConverter());

			// Assert
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeFalse();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Dropped);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBeNull();
			result.Reason.ShouldBe("Bounced Address");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeFalse();
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_delivered_JSON()
		{
			// Arrange

			// Act
			var result = (DeliveredEvent)JsonConvert.DeserializeObject<Event>(DELIVERED_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeTrue();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Delivered);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("127.0.0.1");
			result.Newsletter.ShouldNotBeNull();
			result.Newsletter.Id.ShouldBe("1943530");
			result.Newsletter.SendId.ShouldBe("2308608");
			result.Newsletter.UserListId.ShouldBe("10557865");
			result.Response.ShouldBe("250 OK");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeTrue();
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_click_JSON()
		{
			// Arrange

			// Act
			var result = (ClickEvent)JsonConvert.DeserializeObject<Event>(CLICK_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Click);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("255.255.255.255");
			result.Newsletter.ShouldNotBeNull();
			result.Newsletter.Id.ShouldBe("1943530");
			result.Newsletter.SendId.ShouldBe("2308608");
			result.Newsletter.UserListId.ShouldBe("10557865");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
			result.Url.ShouldBe("http://yourdomain.com/blog/news.html");
			result.UrlOffset.ShouldNotBeNull();
			result.UrlOffset.Index.ShouldBe(0);
			result.UrlOffset.Type.ShouldBe(UrlType.Html);
			result.UserAgent.ShouldBe("Mozilla/5.0 (iPhone; CPU iPhone OS 7_1_2 like Mac OS X) AppleWebKit/537.51.2 (KHTML, like Gecko) Version/7.0 Mobile/11D257 Safari/9537.53");
		}

		[Fact]
		public void Parse_open_JSON()
		{
			// Arrange

			// Act
			var result = (OpenEvent)JsonConvert.DeserializeObject<Event>(OPEN_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Open);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("255.255.255.255");
			result.Newsletter.ShouldNotBeNull();
			result.Newsletter.Id.ShouldBe("1943530");
			result.Newsletter.SendId.ShouldBe("2308608");
			result.Newsletter.UserListId.ShouldBe("10557865");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
			result.UserAgent.ShouldBe("Mozilla/5.0 (Windows NT 5.1; rv:11.0) Gecko Firefox/11.0 (via ggpht.com GoogleImageProxy)");
		}

		[Fact]
		public void Parse_spamreport_JSON()
		{
			// Arrange

			// Act
			var result = (SpamReportEvent)JsonConvert.DeserializeObject<Event>(SPAMREPORT_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.SpamReport);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBeNull();
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
			result.UserAgent.ShouldBeNull();
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
		}

		[Fact]
		public void Parse_unsubscribe_JSON()
		{
			// Arrange

			// Act
			var result = (UnsubscribeEvent)JsonConvert.DeserializeObject<Event>(UNSUBSCRIBE_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Unsubscribe);
			result.InternalEventId.ShouldBeNull();
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBeNull();
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
			result.UserAgent.ShouldBeNull();
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
		}

		[Fact]
		public void Parse_groupunsubscribe_JSON()
		{
			// Arrange

			// Act
			var result = (GroupUnsubscribeEvent)JsonConvert.DeserializeObject<Event>(GROUPUNSUBSCRIBE_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.GroupUnsubscribe);
			result.InternalEventId.ShouldBeNull();
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("255.255.255.255");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
			result.UserAgent.ShouldBe("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
		}

		[Fact]
		public void Parse_groupresubscribe_JSON()
		{
			// Arrange

			// Act
			var result = (GroupResubscribeEvent)JsonConvert.DeserializeObject<Event>(GROUPRESUBSCRIBE_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.GroupResubscribe);
			result.InternalEventId.ShouldBeNull();
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("255.255.255.255");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
			result.UserAgent.ShouldBe("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
		}

		[Fact]
		public async Task Processed()
		{
			// Arrange
			var responseContent = $"[{PROCESSED_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseWebhookEventsAsync(stream).ConfigureAwait(false);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(ProcessedEvent));
			}
		}

		[Fact]
		public async Task Bounced()
		{
			// Arrange
			var responseContent = $"[{BOUNCED_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseWebhookEventsAsync(stream).ConfigureAwait(false);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(BouncedEvent));
			}
		}

		[Fact]
		public async Task Deferred()
		{
			// Arrange
			var responseContent = $"[{DEFERRED_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseWebhookEventsAsync(stream).ConfigureAwait(false);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(DeferredEvent));
			}
		}

		[Fact]
		public async Task Dropped()
		{
			// Arrange
			var responseContent = $"[{DROPPED_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseWebhookEventsAsync(stream).ConfigureAwait(false);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(DroppedEvent));
			}
		}

		[Fact]
		public async Task Click()
		{
			// Arrange
			var responseContent = $"[{CLICK_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseWebhookEventsAsync(stream).ConfigureAwait(false);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(ClickEvent));
			}
		}

		[Fact]
		public async Task Open()
		{
			// Arrange
			var responseContent = $"[{OPEN_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseWebhookEventsAsync(stream).ConfigureAwait(false);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(OpenEvent));
			}
		}

		[Fact]
		public async Task Unsubscribe()
		{
			// Arrange
			var responseContent = $"[{UNSUBSCRIBE_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseWebhookEventsAsync(stream).ConfigureAwait(false);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(UnsubscribeEvent));
			}
		}

		[Fact]
		public async Task GroupUnsubscribe()
		{
			// Arrange
			var responseContent = $"[{GROUPUNSUBSCRIBE_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseWebhookEventsAsync(stream).ConfigureAwait(false);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(GroupUnsubscribeEvent));
			}
		}

		[Fact]
		public async Task GroupResubscribe()
		{
			// Arrange
			var responseContent = $"[{GROUPRESUBSCRIBE_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseWebhookEventsAsync(stream).ConfigureAwait(false);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(GroupResubscribeEvent));
			}
		}

		private Stream GetStream(string responseContent)
		{
			var byteArray = Encoding.UTF8.GetBytes(responseContent);
			var stream = new MemoryStream(byteArray);
			return stream;
		}
	}
}
