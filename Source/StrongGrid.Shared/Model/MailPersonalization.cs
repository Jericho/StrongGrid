using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StrongGrid.Model
{
    public class MailPersonalization
    {
        [JsonProperty("to")]
        public MailAddress[] To { get; set; }

        [JsonProperty("cc")]
        public MailAddress[] CC { get; set; }

        [JsonProperty("bcc")]
        public MailAddress[] BCC { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("headers")]
        public KeyValuePair<string, string>[] Headers { get; set; }

        [JsonProperty("sendat")]
        public DateTime? SendAt { get; set; }
    }
}
