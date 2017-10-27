using Shouldly;
using System.IO;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class WebhookParserTests
	{
		#region FIELDS

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
			using (var stream = new MemoryStream())
			{
				using (var writer = new StreamWriter(stream))
				{
					writer.Write(INBOUND_EMAIL_WEBHOOK);
					writer.Flush();
					stream.Position = 0;

					// Act
					var parser = new WebhookParser();
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
		}
	}
}
