using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DefaultNamespace.Models
{
    [Serializable]
    public class MovieData
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("title")] public string Title;
        [JsonProperty("original_title")] public string Original_title;
        [JsonProperty("overview")] public string Overview;
        [JsonProperty("release_date")] public string release_date;
        [JsonProperty("poster_path")] public string poster_path;  
        [JsonProperty("backdrop_path")] public string backdrop_path;
        [JsonProperty("genre_ids")] public List<int> Genre_Ids;
    }
}