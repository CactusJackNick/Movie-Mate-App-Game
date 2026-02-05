using System.Collections.Generic;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public class ClueFactory : IClueFactory
    {
        public List<ClueData> AssignDataToClues(DetailsSuperlistModel movie, Sprite poster, Sprite backdrop)
        {
            var director = GetDirector(movie);
            var date = movie.Release_Date;
            var tagline = movie.Tagline;
            var genres = GetGenres(movie);
            var actors = GetActors(movie);
            
            var clues = new List<ClueData>
            {
                new() //clue 1
                {
                    _title = "Backdrop",
                    _displayMode = ClueDisplayMode.Backdrop,
                    _imageContext = backdrop
                },
                new() //clue 2
                {
                    _title = "Year & Genres",
                    _displayMode = ClueDisplayMode.Text, 
                    _textContext = $"{date}\n{genres}"
                },
                new() //clue 3
                {
                    _title = "Director",
                    _displayMode = ClueDisplayMode.Text,
                    _textContext = $"{director}"
                },
                new() //clue 4
                {
                    _title = "Actors",
                    _displayMode = ClueDisplayMode.Text,
                    _textContext = $"{actors}"
                },
                new() //clue 5
                {
                    _title = "Quote",
                    _displayMode = ClueDisplayMode.Text,
                    _textContext = $"{tagline}"
                },
                new() //clue 6
                {
                    _title = "Poster",
                    _displayMode = ClueDisplayMode.Poster,
                    _imageContext = poster
                }
            };

            return clues;
        }

        private string GetDirector(DetailsSuperlistModel movie)
        {
            var directorName = string.Empty;

            foreach (var crewMember in movie.Credits.Crew)
            {
                if (crewMember.Job == "Director")
                {
                    directorName = crewMember.NameCrew;
                }
            }
            
            return directorName;
        }
        
        private string GetGenres(DetailsSuperlistModel movie)
        {
            var names = new List<string>();
            foreach (var g in movie.Genres)
            {
                names.Add(g.Name);
            }
            
            return string.Join(", ", names);
        }

        private string GetActors(DetailsSuperlistModel movie)
        {
            var actors = new List<string>();
            foreach (var actor in movie.Credits.Cast)
            {
                if (actor.Acting == "Acting")
                {
                    if (actors.Count >= 3)
                    {
                        break;
                    }
                    actors.Add(actor.ActorName);
                }
            }
            return string.Join("\n", actors);
        }
    }
}