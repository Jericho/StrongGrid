using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Models.Webhooks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class WebhookParserTests
	{
		#region FIELDS

		// PLEASE NOTE: according to SendGrid's documentation, "id" is numerical (i.e.: "id":210).
		// See https://github.com/Jericho/StrongGrid/issues/454 for demonstration that it is in fact a string.
		private const string PROCESSED_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""pool"": {
				""name"":""new_MY_test"",
				""id"":""210""

			},
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""processed"",
			""category"":""cat facts"",
			""sg_event_id"":""rbtnWrG1DVDGGGFHFyun0A=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.000000000000000000000"",
			""asm_group_id"":123456
		}";

		private const string BOUNCED_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""bounce"",
			""category"":""cat facts"",
			""sg_event_id"":""6g4ZI7SA-xmRDv57GoPIPw=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
			""reason"":""500 unknown recipient"",
			""status"":""5.0.0"",
			""type"":""bounce""
		}";

		private const string DEFERRED_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""deferred"",
			""category"":""cat facts"",
			""sg_event_id"":""t7LEShmowp86DTdUW8M-GQ=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
			""response"":""400 try again later"",
			""attempt"":""5""
		}";

		private const string DROPPED_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""dropped"",
			""category"":""cat facts"",
			""sg_event_id"":""zmzJhfJgAfUSOW80yEbPyw=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
			""reason"":""Bounced Address"",
			""status"":""5.0.0""
		}";

		private const string BLOCKED_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""bounce"",
			""category"":""cat facts"",
			""sg_event_id"":""6g4ZI7SA-xmRDv57GoPIPw=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
			""reason"":""500 unknown recipient"",
			""status"":""5.0.0"",
			""type"":""blocked""
		}";

		private const string DELIVERED_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""delivered"",
			""category"":""cat facts"",
			""sg_event_id"":""rWVYmVk90MjZJ9iohOBa3w=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
			""response"":""250 OK""
		}";

		private const string CLICKED_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""click"",
			""category"":""cat facts"",
			""sg_event_id"":""kCAi1KttyQdEKHhdC-nuEA=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
			""useragent"":""Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"",
			""ip"":""255.255.255.255"",
			""url"":""http://www.sendgrid.com/""
		}";

		private const string OPENED_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""open"",
			""sg_machine_open"": false,
			""category"":""cat facts"",
			""sg_event_id"":""FOTFFO0ecsBE-zxFXfs6WA=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
			""useragent"":""Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"",
			""ip"":""255.255.255.255""
		}";

		private const string SPAMREPORT_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""spamreport"",
			""category"":""cat facts"",
			""sg_event_id"":""37nvH5QBz858KGVYCM4uOA=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0""
		}";

		private const string UNSUBSCRIBE_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""unsubscribe"",
			""category"":""cat facts"",
			""sg_event_id"":""zz_BjPgU_5pS-J8vlfB1sg=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0""
		}";

		private const string GROUPUNSUBSCRIBE_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""group_unsubscribe"",
			""category"":""cat facts"",
			""sg_event_id"":""ahSCB7xYcXFb-hEaawsPRw=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
			""useragent"":""Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"",
			""ip"":""255.255.255.255"",
			""url"":""http://www.sendgrid.com/"",
			""asm_group_id"":10
		}";

		private const string GROUPRESUBSCRIBE_JSON = @"
		{
			""email"":""example@test.com"",
			""timestamp"":1513299569,
			""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
			""event"":""group_resubscribe"",
			""category"":""cat facts"",
			""sg_event_id"":""w_u0vJhLT-OFfprar5N93g=="",
			""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
			""useragent"":""Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"",
			""ip"":""255.255.255.255"",
			""url"":""http://www.sendgrid.com/"",
			""asm_group_id"":10
		}";

		private const string INBOUND_EMAIL_WEBHOOK = @"--xYzZY
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

		private const string INBOUND_EMAIL_UNUSUAL_ENCODING_WEBHOOK = @"--xYzZY
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

{""to"":""UTF-8"",""html"":""us-ascii"",""subject"":""UTF-8"",""from"":""UTF-8"",""text"":""iso-8859-10""}
--xYzZY
Content-Disposition: form-data; name=""SPF""

softfail

--xYzZY
Content-Disposition: form-data; name=""to""

""Test Recipient"" <test@api.yourdomain.com>
--xYzZY
Content-Disposition: form-data; name=""html""

<html><body><strong>Hello SendGrid!</body></html>

--xYzZY
Content-Disposition: form-data; name=""from""

Bob Smith<bob@example.com>
--xYzZY
Content-Disposition: form-data; name=""text""

Hello SendGrid!

--xYzZY
Content-Disposition: form-data; name=""sender_ip""

10.43.24.23
--xYzZY
Content-Disposition: form-data; name=""attachments""

0
--xYzZY--";

		// I obtained the sample payload, signature and timestamp by invoking
		// await strongGridClient.WebhookSettings.SendEventTestAsync("... my url ...");
		private const string SAMPLE_PUBLIC_KEY = "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE2is1eViXeZ9NwNbYKD/b51+WBZQVf+mLT0QCLiD6+HgWlNkrldvci/3m/o72GgCr3ilINxo9FpHElSHNnlYA7A==";
		private const string SAMPLE_PAYLOAD = "[{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"processed\",\"category\":[\"cat facts\"],\"sg_event_id\":\"m-8gndi_kLAVOUXWX79vdg==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\"},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"deferred\",\"category\":[\"cat facts\"],\"sg_event_id\":\"Xkjq_rPYT2IV1ZTNPx2gGQ==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\",\"response\":\"400 try again later\",\"attempt\":\"5\"},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"delivered\",\"category\":[\"cat facts\"],\"sg_event_id\":\"lNsPF9LUYT70RfvIabpolA==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\",\"response\":\"250 OK\"},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"open\",\"category\":[\"cat facts\"],\"sg_event_id\":\"WElX21DXbk0bn2S1p_E_gA==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\",\"useragent\":\"Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)\",\"ip\":\"255.255.255.255\"},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"click\",\"category\":[\"cat facts\"],\"sg_event_id\":\"BC5Jcj5IdtB5JzctXdF7jQ==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\",\"useragent\":\"Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)\",\"ip\":\"255.255.255.255\",\"url\":\"http://www.sendgrid.com/\"},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"bounce\",\"category\":[\"cat facts\"],\"sg_event_id\":\"1jABOEVCpBNStQg2Ji-Pvw==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\",\"reason\":\"500 unknown recipient\",\"status\":\"5.0.0\"},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"dropped\",\"category\":[\"cat facts\"],\"sg_event_id\":\"N0_KKSkckt9hUtasadYffA==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\",\"reason\":\"Bounced Address\",\"status\":\"5.0.0\"},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"spamreport\",\"category\":[\"cat facts\"],\"sg_event_id\":\"ZvWbMhp8tWTEFUqe4J4FHg==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\"},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"unsubscribe\",\"category\":[\"cat facts\"],\"sg_event_id\":\"CWoMUe_SCatUCphHVjbLUw==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\"},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"group_unsubscribe\",\"category\":[\"cat facts\"],\"sg_event_id\":\"oGga2Bjm_hPpr7Ws7U6z7Q==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\",\"useragent\":\"Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)\",\"ip\":\"255.255.255.255\",\"url\":\"http://www.sendgrid.com/\",\"asm_group_id\":10},\r\n" +
			"{\"email\":\"example@test.com\",\"timestamp\":1606575333,\"smtp-id\":\"\\u003c14c5d75ce93.dfd.64b469@ismtpd-555\\u003e\",\"event\":\"group_resubscribe\",\"category\":[\"cat facts\"],\"sg_event_id\":\"h5eCKbTzFfA7vy8L1teZIg==\",\"sg_message_id\":\"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0\",\"useragent\":\"Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)\",\"ip\":\"255.255.255.255\",\"url\":\"http://www.sendgrid.com/\",\"asm_group_id\":10}]\r\n";
		private const string SAMPLE_SIGNATURE = "MEYCIQC7zQwrnBHCHxGZOkh70LJ144tIuSOxru0YGCn/bSE0/QIhAJ22PAa8HdZZQ+wrK+9sgeWF8BDdd1t+zbmCLO33RcyA";
		private const string SAMPLE_TIMESTAMP = "1606575372";

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
				inboundEmail.Text.Replace("\r\n", "\n").ShouldBe("Test #1\n");
				inboundEmail.To.ShouldNotBeNull();
				inboundEmail.To.Length.ShouldBe(1);
				inboundEmail.To[0].Email.ShouldBe("test@api.yourdomain.com");
				inboundEmail.To[0].Name.ShouldBe("Test Recipient");
			}
		}

		[Fact]
		public async Task InboundEmailAsync()
		{
			// Arrange
			var parser = new WebhookParser();
			using (var stream = GetStream(INBOUND_EMAIL_WEBHOOK))
			{
				// Act
				var inboundEmail = await parser.ParseInboundEmailWebhookAsync(stream);

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
				inboundEmail.Text.Replace("\r\n", "\n").ShouldBe("Test #1\n");
				inboundEmail.To.ShouldNotBeNull();
				inboundEmail.To.Length.ShouldBe(1);
				inboundEmail.To[0].Email.ShouldBe("test@api.yourdomain.com");
				inboundEmail.To[0].Name.ShouldBe("Test Recipient");
			}
		}

		[Fact]
		public async Task InboundEmailWithAttachmentsAsync()
		{
			using (var fileStream = File.OpenRead("InboudEmailTestData/email_with_attachments.txt"))
			{
				var parser = new WebhookParser();
				var inboundEmail = await parser.ParseInboundEmailWebhookAsync(fileStream);

				inboundEmail.ShouldNotBeNull();

				inboundEmail.Attachments.ShouldNotBeNull();
				inboundEmail.Attachments.Length.ShouldBe(2);
				inboundEmail.Attachments[0].ContentId.ShouldBeNull();
				inboundEmail.Attachments[0].ContentType.ShouldBe("image/jpeg");
				inboundEmail.Attachments[0].FileName.ShouldBe("001.jpg");
				inboundEmail.Attachments[0].Id.ShouldBe("attachment2");
				inboundEmail.Attachments[0].Name.ShouldBe("001.jpg");
				inboundEmail.Attachments[1].ContentId.ShouldBe("image001.png@01D85101.00357470");
				inboundEmail.Attachments[1].ContentType.ShouldBe("image/png");
				inboundEmail.Attachments[1].FileName.ShouldBe("image001.png");
				inboundEmail.Attachments[1].Id.ShouldBe("attachment1");
				inboundEmail.Attachments[1].Name.ShouldBe("image001.png");

				inboundEmail.Dkim.ShouldBe("{@hotmail.com : pass}");

				inboundEmail.To[0].Email.ShouldBe("test@api.mydomain.com");
				inboundEmail.To[0].Name.ShouldBe("API test user");

				inboundEmail.Cc.Length.ShouldBe(0);

				inboundEmail.From.Email.ShouldBe("test@example.com");
				inboundEmail.From.Name.ShouldBe("Test User");

				inboundEmail.SenderIp.ShouldBe("40.92.19.62");

				inboundEmail.SpamReport.ShouldBeNull();

				inboundEmail.Envelope.From.ShouldBe("test@example.com");
				inboundEmail.Envelope.To.Length.ShouldBe(1);
				inboundEmail.Envelope.To.ShouldContain("test@api.mydomain.com");

				inboundEmail.Subject.ShouldBe("Test #1");

				inboundEmail.SpamScore.ShouldBeNull();

				inboundEmail.Charsets.Except(new[]
				{
					new KeyValuePair<string, string>("to", "UTF-8"),
					new KeyValuePair<string, string>("filename", "UTF-8"),
					new KeyValuePair<string, string>("html", "us-ascii"),
					new KeyValuePair<string, string>("subject", "UTF-8"),
					new KeyValuePair<string, string>("from", "UTF-8"),
					new KeyValuePair<string, string>("text", "us-ascii"),
				}).Count().ShouldBe(0);

				inboundEmail.Spf.ShouldBe("pass");
			}
		}

		[Fact]
		public async Task InboundEmail_with_unusual_encoding()
		{
			// Arrange
			var parser = new WebhookParser();
			using (var stream = GetStream(INBOUND_EMAIL_UNUSUAL_ENCODING_WEBHOOK))
			{
				// Act
				var inboundEmail = await parser.ParseInboundEmailWebhookAsync(stream);

				// Assert
				inboundEmail.Charsets.ShouldNotBeNull();
				inboundEmail.Charsets.Except(new[]
				{
					new KeyValuePair<string, string>("to", "UTF-8"),
					new KeyValuePair<string, string>("subject", "UTF-8"),
					new KeyValuePair<string, string>("from", "UTF-8"),
					new KeyValuePair<string, string>("html", "us-ascii"),
					new KeyValuePair<string, string>("text", "iso-8859-10")
				}).Count().ShouldBe(0);
				inboundEmail.Text.Replace("\r\n", "\n").ShouldBe("Hello SendGrid!\n");
			}
		}

		[Fact]
		public async Task InboundEmailRawContentAsync()
		{
			// Arrange
			var parser = new WebhookParser();
			using (var fileStream = File.OpenRead("InboudEmailTestData/raw_email.txt"))
			{
				// Act
				var inboundEmail = await parser.ParseInboundEmailWebhookAsync(fileStream);

				// Assert
				inboundEmail.Charsets.ShouldNotBeNull();
				inboundEmail.Charsets.Except(new[]
				{
					new KeyValuePair<string, string>("to", "UTF-8"),
					new KeyValuePair<string, string>("subject", "UTF-8"),
					new KeyValuePair<string, string>("from", "UTF-8"),
				}).Count().ShouldBe(0);
				inboundEmail.Text.ShouldStartWith("This is a test");
				inboundEmail.Attachments.ShouldNotBeNull();
				inboundEmail.Attachments.Length.ShouldBe(1);
				inboundEmail.Attachments[0].FileName.ShouldBe("Frangipani Flowers.jpg");
			}
		}

		[Fact]
		public async Task InboundEmailRawContent_with_attachments_Async()
		{
			// Arrange
			var parser = new WebhookParser();
			using (var fileStream = File.OpenRead("InboudEmailTestData/raw_email_with_attachments.txt"))
			{
				// Act
				var inboundEmail = await parser.ParseInboundEmailWebhookAsync(fileStream);

				// Assert
				inboundEmail.Charsets.ShouldNotBeNull();
				inboundEmail.Charsets.Except(new[]
				{
					new KeyValuePair<string, string>("to", "UTF-8"),
					new KeyValuePair<string, string>("subject", "UTF-8"),
					new KeyValuePair<string, string>("from", "UTF-8"),
				}).Count().ShouldBe(0);
				inboundEmail.Text.ShouldStartWith("Hello World");
				inboundEmail.Attachments.ShouldNotBeNull();
				inboundEmail.Attachments.Length.ShouldBe(2);
				inboundEmail.Attachments[0].Name.ShouldStartWith("48 hr Flash-Save 25%");
				inboundEmail.Attachments[1].Name.ShouldBe("DesertSunrise.jpg");
			}
		}

		[Fact]
		public void Parse_processed_JSON()
		{
			// Arrange

			// Act
			var result = (ProcessedEvent)JsonSerializer.Deserialize<Event>(PROCESSED_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.AsmGroupId.ShouldBe(123456);
			result.Categories.Length.ShouldBe(1);
			result.Categories[0].ShouldBe("cat facts");
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.Processed);
			result.InternalEventId.ShouldBe("rbtnWrG1DVDGGGFHFyun0A==");
			result.InternalMessageId.ShouldBe("14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.000000000000000000000");
			result.IpPool.ShouldNotBeNull();
			result.IpPool.Id.ShouldBe("210");
			result.IpPool.Name.ShouldBe("new_MY_test");
			result.MessageId.ShouldBe("14c5d75ce93.dfd.64b469");
			result.SmtpId.ShouldBe("<14c5d75ce93.dfd.64b469@ismtpd-555>");
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_bounced_JSON()
		{
			// Arrange

			// Act
			BouncedEvent result = (BouncedEvent)JsonSerializer.Deserialize<Event>(BOUNCED_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Categories.Length.ShouldBe(1);
			result.Categories[0].ShouldBe("cat facts");
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.Bounce);
			result.InternalEventId.ShouldBe("6g4ZI7SA-xmRDv57GoPIPw==");
			result.InternalMessageId.ShouldBe("14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0");
			result.MessageId.ShouldBe("14c5d75ce93.dfd.64b469");
			result.Reason.ShouldBe("500 unknown recipient");
			result.SmtpId.ShouldBe("<14c5d75ce93.dfd.64b469@ismtpd-555>");
			result.Status.ShouldBe("5.0.0");
			result.Type.ShouldBe(BounceType.Bounce);
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_deferred_JSON()
		{
			// Arrange

			// Act
			var result = (DeferredEvent)JsonSerializer.Deserialize<Event>(DEFERRED_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.AsmGroupId.ShouldBeNull();
			result.Attempts.ShouldBe(5);
			result.Categories.Length.ShouldBe(1);
			result.Categories[0].ShouldBe("cat facts");
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.Deferred);
			result.InternalEventId.ShouldBe("t7LEShmowp86DTdUW8M-GQ==");
			result.InternalMessageId.ShouldBe("14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0");
			result.MessageId.ShouldBe("14c5d75ce93.dfd.64b469");
			result.SmtpId.ShouldBe("<14c5d75ce93.dfd.64b469@ismtpd-555>");
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_dropped_JSON()
		{
			// Arrange

			// Act
			var result = (DroppedEvent)JsonSerializer.Deserialize<Event>(DROPPED_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Categories.Length.ShouldBe(1);
			result.Categories[0].ShouldBe("cat facts");
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.Dropped);
			result.InternalEventId.ShouldBe("zmzJhfJgAfUSOW80yEbPyw==");
			result.InternalMessageId.ShouldBe("14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0");
			result.MessageId.ShouldBe("14c5d75ce93.dfd.64b469");
			result.Reason.ShouldBe("Bounced Address");
			result.SmtpId.ShouldBe("<14c5d75ce93.dfd.64b469@ismtpd-555>");
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_blocked_JSON()
		{
			// Arrange

			// Act
			var result = (BouncedEvent)JsonSerializer.Deserialize<Event>(BLOCKED_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Categories.Length.ShouldBe(1);
			result.Categories[0].ShouldBe("cat facts");
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.Bounce);
			result.InternalEventId.ShouldBe("6g4ZI7SA-xmRDv57GoPIPw==");
			result.InternalMessageId.ShouldBe("14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0");
			result.MessageId.ShouldBe("14c5d75ce93.dfd.64b469");
			result.Reason.ShouldBe("500 unknown recipient");
			result.SmtpId.ShouldBe("<14c5d75ce93.dfd.64b469@ismtpd-555>");
			result.Status.ShouldBe("5.0.0");
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.Type.ShouldBe(BounceType.Blocked);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_delivered_JSON()
		{
			// Arrange

			// Act
			var result = (DeliveredEvent)JsonSerializer.Deserialize<Event>(DELIVERED_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.AsmGroupId.ShouldBeNull();
			result.Categories.Length.ShouldBe(1);
			result.Categories[0].ShouldBe("cat facts");
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.Delivered);
			result.InternalEventId.ShouldBe("rWVYmVk90MjZJ9iohOBa3w==");
			result.InternalMessageId.ShouldBe("14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0");
			result.MessageId.ShouldBe("14c5d75ce93.dfd.64b469");
			result.Response.ShouldBe("250 OK");
			result.SmtpId.ShouldBe("<14c5d75ce93.dfd.64b469@ismtpd-555>");
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_clicked_JSON()
		{
			// Arrange

			// Act
			var result = (ClickedEvent)JsonSerializer.Deserialize<Event>(CLICKED_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Categories.Length.ShouldBe(1);
			result.Categories[0].ShouldBe("cat facts");
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.Click);
			result.InternalEventId.ShouldBe("kCAi1KttyQdEKHhdC-nuEA==");
			result.InternalMessageId.ShouldBe("14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0");
			result.IpAddress.ShouldBe("255.255.255.255");
			result.MessageId.ShouldBe("14c5d75ce93.dfd.64b469");
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.Url.ShouldBe("http://www.sendgrid.com/");
			result.UserAgent.ShouldBe("Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_opened_JSON()
		{
			// Arrange

			// Act
			var result = (OpenedEvent)JsonSerializer.Deserialize<Event>(OPENED_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.Open);
			result.MachineOpen.ShouldBeFalse();
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UserAgent.ShouldBe("Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_spamreport_JSON()
		{
			// Arrange

			// Act
			var result = (SpamReportEvent)JsonSerializer.Deserialize<Event>(SPAMREPORT_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.SpamReport);
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_unsubscribe_JSON()
		{
			// Arrange

			// Act
			var result = (UnsubscribeEvent)JsonSerializer.Deserialize<Event>(UNSUBSCRIBE_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.Unsubscribe);
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_groupunsubscribe_JSON()
		{
			// Arrange

			// Act
			var result = (GroupUnsubscribeEvent)JsonSerializer.Deserialize<Event>(GROUPUNSUBSCRIBE_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.GroupUnsubscribe);
			result.IpAddress.ShouldBe("255.255.255.255");
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UserAgent.ShouldBe("Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
		}

		[Fact]
		public void Parse_groupresubscribe_JSON()
		{
			// Arrange

			// Act
			var result = (GroupResubscribeEvent)JsonSerializer.Deserialize<Event>(GROUPRESUBSCRIBE_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Email.ShouldBe("example@test.com");
			result.EventType.ShouldBe(EventType.GroupResubscribe);
			result.IpAddress.ShouldBe("255.255.255.255");
			result.Timestamp.ToUnixTime().ShouldBe(1513299569);
			result.UserAgent.ShouldBe("Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(0);
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
				var result = await parser.ParseEventsWebhookAsync(stream);

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
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(BouncedEvent));
				result[0].EventType.ShouldBe(EventType.Bounce);
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
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(DeferredEvent));
				result[0].EventType.ShouldBe(EventType.Deferred);
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
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(DroppedEvent));
				result[0].EventType.ShouldBe(EventType.Dropped);
			}
		}

		[Fact]
		public async Task Clicked()
		{
			// Arrange
			var responseContent = $"[{CLICKED_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(ClickedEvent));
				result[0].EventType.ShouldBe(EventType.Click);
			}
		}

		[Fact]
		public async Task Opened()
		{
			// Arrange
			var responseContent = $"[{OPENED_JSON}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(OpenedEvent));
				result[0].EventType.ShouldBe(EventType.Open);
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
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(UnsubscribeEvent));
				result[0].EventType.ShouldBe(EventType.Unsubscribe);
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
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(GroupUnsubscribeEvent));
				result[0].EventType.ShouldBe(EventType.GroupUnsubscribe);
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
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].GetType().ShouldBe(typeof(GroupResubscribeEvent));
				result[0].EventType.ShouldBe(EventType.GroupResubscribe);
			}
		}

		[Fact]
		public void ValidateWebhookSignature()
		{
			// Arrange
			var parser = new WebhookParser();

			// Act
			var result = parser.ParseSignedEventsWebhook(SAMPLE_PAYLOAD, SAMPLE_PUBLIC_KEY, SAMPLE_SIGNATURE, SAMPLE_TIMESTAMP);

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(11); // The sample payload contains 11 events
		}

		[Fact]
		// This unit test validates that we fixed the issue described in GH-492.
		// It proves that we can serialize and deserilize events without losing
		// properties of derived types such as 'Reason' for a bounced event,
		// 'UserAgent' for a click event and 'IpAddress' for an open event.
		public async Task Serialize_and_deserialize_derived_types()
		{
			// Arrange
			var events = new Event[]
			{
				new BouncedEvent() { Email = "test1@whatever.com", Reason = "This is a test"},
				new ClickedEvent() { Email = "test2@whatever.com", UserAgent = "This is a test" },
				new OpenedEvent() { Email = "test3@whatever.com", IpAddress = "This is a test" }
			};

			// Act
			var serializedEvents = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(events));
			var result = await JsonSerializer.DeserializeAsync<Event[]>(serializedEvents);

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);

			result[0].Email.ShouldBe("test1@whatever.com");
			result[0].EventType.ShouldBe(EventType.Bounce);
			((BouncedEvent)result[0]).Reason.ShouldBe("This is a test");

			result[1].Email.ShouldBe("test2@whatever.com");
			result[1].EventType.ShouldBe(EventType.Click);
			((ClickedEvent)result[1]).UserAgent.ShouldBe("This is a test");

			result[2].Email.ShouldBe("test3@whatever.com");
			result[2].EventType.ShouldBe(EventType.Open);
			((OpenedEvent)result[2]).IpAddress.ShouldBe("This is a test");
		}

		[Fact]
		// This unit test reproduces the problem described in GH-491 and
		// demonstrates that it was resolved in StrongGrid 0.99.1
		public async Task Webhook_includes_unknow_property()
		{
			const string JSON_WITH_RESELLER_ID = @"
			{
				""reseller_id"":1234,
				""email"":""example@test.com"",
				""timestamp"":1513299569,
				""smtp-id"":""<14c5d75ce93.dfd.64b469@ismtpd-555>"",
				""event"":""open"",
				""sg_machine_open"": false,
				""category"":""cat facts"",
				""sg_event_id"":""FOTFFO0ecsBE-zxFXfs6WA=="",
				""sg_message_id"":""14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"",
				""useragent"":""Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"",
				""ip"":""255.255.255.255""
			}";

			// Arrange
			var responseContent = $"[{JSON_WITH_RESELLER_ID}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(responseContent))
			{
				// Act
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].UniqueArguments.ShouldNotBeNull();
				result[0].UniqueArguments.Count.ShouldBe(1);
				result[0].UniqueArguments.ShouldContainKeyAndValue("reseller_id", "1234"); // Note that the value has been converted to a string
			}
		}

		[Fact]
		// This unit test validates that we fixed the issue described in GH-493.
		public async Task UniqueArguments_are_serialized()
		{
			// Arrange
			Event bouncedEvent = new BouncedEvent()
			{
				Email = "test1@whatever.com",
				Reason = "This is a test"
			};
			bouncedEvent.UniqueArguments.Add("aaa", "1111");
			bouncedEvent.UniqueArguments.Add("bbb", "qwerty");

			// Act
			var ms = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(bouncedEvent));
			var result = await JsonSerializer.DeserializeAsync<Event>(ms);

			// Assert
			result.ShouldNotBeNull();
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(2);
			result.UniqueArguments.ShouldContainKeyAndValue("aaa", "1111");
			result.UniqueArguments.ShouldContainKeyAndValue("bbb", "qwerty");
		}

		[Theory]
		// This unit test demonstrates that we can handle the various message Id formats, as described in GH-504.
		[InlineData("this-is-the-messageId.filterdrecv-blablabla", "this-is-the-messageId")]// This was the format until January 2024
		[InlineData("this-is-the-messageId.recvd-blablabla", "this-is-the-messageId")]// This format was introduced in January 2024
		[InlineData("this-is-the-messageId.SomeOtherSeparator-blablabla", "this-is-the-messageId")] // In case SendGrid changes the separator to some other arbitrary value at some point in the future
		[InlineData("this-messageId-does-not-contain-any-separator", "this-messageId-does-not-contain-any-separator")] // This is to validate that we can handle the case where messageId does not contain any of the known separators
		public async Task Can_handle_various_message_id_separators(string internalMessageId, string expectedMesageId)
		{
			var jsonPayload = $"[{{\"event\":\"processed\",\r\n\"sg_message_id\":\"{internalMessageId}\"}}]";
			var parser = new WebhookParser();
			using (var stream = GetStream(jsonPayload))
			{
				// Act
				var result = await parser.ParseEventsWebhookAsync(stream);

				// Assert
				result.ShouldNotBeNull();
				result.Length.ShouldBe(1);
				result[0].MessageId.ShouldBe(expectedMesageId);
			}

		}

		private static Stream GetStream(string responseContent)
		{
			var byteArray = Encoding.UTF8.GetBytes(responseContent);
			var stream = new MemoryStream(byteArray);
			return stream;
		}
	}
}
