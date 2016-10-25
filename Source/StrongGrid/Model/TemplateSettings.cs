using Newtonsoft.Json;

namespace StrongGrid.Model
{
    public class TemplateSettings : Setting
    {
        [JsonProperty("html_content")]
        public string HtmlContent { get; set; }
    }
}
