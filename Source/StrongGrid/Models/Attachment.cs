using HeyRed.Mime;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace StrongGrid.Models
{
	/// <summary>
	/// An email attachment.
	/// </summary>
	public class Attachment
	{
		// SendGrid doesn't limit the size of an attachment, but they limit the total size of an email to 30MB
		// Therefore it's safe to assume that a given attachment cannot be larger than 30MB
		private const int MAX_ATTACHMENT_SIZE = 30 * 1024 * 1024;

		// Reasonable 4kb buffer when reading from stream
		private const int BUFFER_SIZE = 4096;

		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>
		/// The content.
		/// </value>
		[JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
		public string Content { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>
		/// The name of the file.
		/// </value>
		[JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets the disposition.
		/// </summary>
		/// <value>
		/// The disposition.
		/// </value>
		[JsonProperty("disposition", NullValueHandling = NullValueHandling.Ignore)]
		public string Disposition { get; set; }

		/// <summary>
		/// Gets or sets the content identifier.
		/// </summary>
		/// <value>
		/// The content identifier.
		/// </value>
		[JsonProperty("content_id", NullValueHandling = NullValueHandling.Ignore)]
		public string ContentId { get; set; }

		/// <summary>
		/// Convenience method that creates an attachment from a local file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment.</returns>
		/// <exception cref="FileNotFoundException">Unable to find the local file.</exception>
		/// <exception cref="Exception">File exceeds the size limit.</exception>
		public static Attachment FromLocalFile(string filePath, string mimeType = null, string contentId = null)
		{
			var fileName = Path.GetFileName(filePath);
			using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				if (fileStream.Length > MAX_ATTACHMENT_SIZE) throw new Exception("File exceeds the size limit");
				return FromStream(fileStream, fileName, mimeType, contentId);
			}
		}

		/// <summary>
		/// Convenience method that creates an attachment from a stream.
		/// </summary>
		/// <param name="contentStream">The stream.</param>
		/// <param name="fileName">The name of the attachment.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment.</returns>
		/// <exception cref="ArgumentNullException">contentStream is null.</exception>
		/// <exception cref="ArgumentException">The content stream is not readable.</exception>
		/// <exception cref="ArgumentException">The content is empty.</exception>
		/// <exception cref="Exception">Content exceeds the size limit.</exception>
		public static Attachment FromStream(Stream contentStream, string fileName, string mimeType = null, string contentId = null)
		{
			if (contentStream == null) throw new ArgumentNullException(nameof(contentStream));
			if (!contentStream.CanRead) throw new ArgumentException("The content stream is not readable", nameof(contentStream));

			using (var base64ContentStream = new CryptoStream(contentStream, new ToBase64Transform(), CryptoStreamMode.Read))
			using (var reader = new StreamReader(base64ContentStream))
			{
				return FromBase64String(reader.ReadToEnd(), fileName, mimeType, contentId);
			}
		}

		/// <summary>
		/// Convenience method that creates an attachment from a stream.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="fileName">The name of the attachment.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment.</returns>
		/// <exception cref="ArgumentNullException">content is null.</exception>
		/// <exception cref="ArgumentException">The content is empty.</exception>
		/// <exception cref="Exception">File exceeds the size limit.</exception>
		public static Attachment FromBinary(byte[] content, string fileName, string mimeType = null, string contentId = null)
		{
			if (content == null) throw new ArgumentNullException(nameof(content));
			if (content.Length == 0) throw new ArgumentException("The content is empty", nameof(content));

			return FromBase64String(Convert.ToBase64String(content), fileName, mimeType, contentId);
		}

		/// <summary>
		/// Convenience method that creates an attachment from a string.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="fileName">The name of the attachment.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment.</returns>
		/// <exception cref="ArgumentNullException">content is null.</exception>
		/// <exception cref="ArgumentException">The content is empty.</exception>
		/// <exception cref="Exception">File exceeds the size limit.</exception>
		public static Attachment FromString(string content, string fileName, string mimeType = null, string contentId = null)
		{
			if (content == null) throw new ArgumentNullException(nameof(content));
			if (content.Length == 0) throw new ArgumentException("The content is empty", nameof(content));
			return FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(content)), fileName, mimeType, contentId);
		}

		/// <summary>
		/// Convenience method that creates an attachment from a base64 encoded string.
		/// </summary>
		/// <param name="content">The base64 encoded content.</param>
		/// <param name="fileName">The name of the attachment.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment.</returns>
		/// <exception cref="ArgumentException">The content is empty.</exception>
		/// <exception cref="Exception">Content exceeds the size limit.</exception>
		public static Attachment FromBase64String(string content, string fileName, string mimeType = null, string contentId = null)
		{
			if (content == null) throw new ArgumentNullException(nameof(content));
			if (content.Length == 0) throw new ArgumentException("The content is empty", nameof(content));
			if (content.Length > MAX_ATTACHMENT_SIZE) throw new Exception("Content exceeds the size limit");

			if (string.IsNullOrEmpty(mimeType))
			{
				mimeType = MimeTypesMap.GetMimeType(fileName);
			}

			return new Attachment()
			{
				Content = content,
				ContentId = contentId,
				Disposition = string.IsNullOrEmpty(contentId) ? "attachment" : "inline",
				FileName = fileName,
				Type = mimeType
			};
		}
	}
}
