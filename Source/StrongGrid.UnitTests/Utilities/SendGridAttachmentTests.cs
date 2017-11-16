using Shouldly;
using StrongGrid.Utilities;
using System;
using System.Text;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class SendGridAttachmentTests
	{
		[Fact]
		public void Large_binary_throws()
		{
			// Arrange
			var content = new byte[31 * 1024 * 1024];
			var fileName = "LargeFile.txt";

			// Act
			var result = Should.Throw<Exception>(() => SendGridAttachment.FromBinary(content, fileName));

			// Assert
			result.Message.ShouldBe("Attachment exceeds the size limit");
		}

		[Fact]
		public void Small_binary_attachment()
		{
			// Arrange
			var content = Encoding.UTF8.GetBytes("Hello World!");
			var fileName = "SmallFile.txt";

			// Act
			var result = SendGridAttachment.FromBinary(content, fileName);

			// Assert
			result.Content = Convert.ToBase64String(content);
			result.ContentId = null;
			result.Disposition = "attachment";
			result.FileName = "SmallFile.txt";
			result.Type = "text/plain";
		}

		[Fact]
		public void Small_binary_inline()
		{
			// Arrange
			var content = Encoding.UTF8.GetBytes("Hello World!");
			var fileName = "SmallFile.txt";
			var contentId = "abc123";

			// Act
			var result = SendGridAttachment.FromBinary(content, fileName, contentId: contentId);

			// Assert
			result.Content = Convert.ToBase64String(content);
			result.ContentId = contentId;
			result.Disposition = "inline";
			result.FileName = "SmallFile.txt";
			result.Type = "text/plain";
		}
	}
}
