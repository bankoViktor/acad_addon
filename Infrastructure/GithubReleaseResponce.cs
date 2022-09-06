using Newtonsoft.Json;
using System;

namespace Addon.Infrastructure
{
    internal class GithubReleaseResponce
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedDate { get; set; }

        [JsonProperty("assets")]
        public GithubReleaseResponceAsset[] Assets { get; set; }
    }

    internal class GithubReleaseResponceAsset
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("browser_download_url")]
        public string DownloadUrl { get; set; }
    }
}
