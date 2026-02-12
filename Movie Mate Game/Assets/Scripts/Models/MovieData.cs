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
        [JsonProperty("overview")] public string Overview;
        //[JsonProperty("release_date")] public string Release_date;
        [JsonProperty("poster_path")] public string Poster_path;  
        [JsonProperty("backdrop_path")] public string Backdrop_Path;
        [JsonProperty("vote_average")] public float Vote_average;
        [JsonProperty("genre_ids")] public List<int> Genre_Ids;
    }
}