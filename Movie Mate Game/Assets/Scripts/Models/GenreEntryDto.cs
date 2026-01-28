using System;
using Newtonsoft.Json;

namespace DefaultNamespace.Models
{
    [Serializable]
    public class GenreEntryDto
    { 
        [JsonProperty("id")] public int Id;
        [JsonProperty("name")] public string Name;
    }
}