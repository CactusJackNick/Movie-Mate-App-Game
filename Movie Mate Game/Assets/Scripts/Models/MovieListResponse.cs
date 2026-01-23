using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DefaultNamespace.Models
{
    [Serializable]
    public class MovieListResponse
    {
        [JsonProperty("page")] private int Page;
        [JsonProperty("results")] public List<MovieData> Results;
    }
}