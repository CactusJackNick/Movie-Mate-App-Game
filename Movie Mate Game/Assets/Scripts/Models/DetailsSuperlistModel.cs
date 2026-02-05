
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DefaultNamespace.Models
{
    [Serializable]
    public class DetailsSuperlistModel
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("title")] public string Title;
        [JsonProperty("overview")] public string Overview;
        [JsonProperty("release_date")] public string Release_Date;
        [JsonProperty("poster_path")] public string PosterPath;
        [JsonProperty("backdrop_path")] public string BackdropPath;
        [JsonProperty("vote_average")] public float Vote_Average;  
        [JsonProperty("tagline")] public string Tagline;
        
        [JsonProperty("credits")] public Credits Credits; 
        [JsonProperty("genres")] public List<GenreEntryDto> Genres;
    }

    [Serializable]
    public class Credits
    {
        [JsonProperty("crew")] public List<CrewMember> Crew;
        [JsonProperty("cast")] public List<CastMember> Cast;
    }

    [Serializable]
    public class CrewMember
    {
        [JsonProperty("name")] public string NameCrew;
        [JsonProperty("job")] public string Job;        
        [JsonProperty("department")] public string Department;
    }

    [Serializable]
    public class CastMember
    {
        [JsonProperty("id")] public int ActorId;
        [JsonProperty("name")] public string ActorName;
        [JsonProperty("known_for_department")] public string Acting;
    }
    
    [Serializable]
    public class GenreEntryDto
    { 
        [JsonProperty("id")] public int Id;
        [JsonProperty("name")] public string Name;
    }
}