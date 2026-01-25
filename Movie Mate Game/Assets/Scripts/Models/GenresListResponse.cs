using System.Collections.Generic;
using Newtonsoft.Json;

namespace DefaultNamespace.Models
{
    public class GenresListResponse
    {
        [JsonProperty("genres")] public List<GenreEntryDto> Genres;
    }
}