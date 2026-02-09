using System.Collections.Generic;
using DefaultNamespace.Models;

namespace DefaultNamespace.Game
{
    public class FeedbackService : IFeedbackService
    {
        public GuessResultModel EvaluateGuess(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            return new GuessResultModel
            {
                DirectorName = GetDirectorName(guess),
                DirectorColor = GetDirectorColor(guess, target),
                
                ActorsText = GetActorsText(guess),
                ActorsColor = GetActorsColor(guess, target),
                
                GenresText = GetGenresText(guess),
                GenresColor = GetGenresColor(guess, target),
                
                YearText = GetYearText(guess),
                YearColor = GetYearColor(guess, target),
                RotateYearArrow = CompareYearsToDetermineArrowRot(guess, target)
            };
        }
        
        private string GetDirectorName(DetailsSuperlistModel model)
        {
            var directorName = "";
            foreach (var person in model.Credits.Crew)
            {
                if (person.Job == "Director")
                {
                   directorName = person.NameCrew;
                }
            }
            
            return string.IsNullOrEmpty(directorName)
                ? "N/A" 
                : directorName;
        }

        private FeedbackColor GetDirectorColor(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessDir = GetDirectorName(guess);
            var targetDir = GetDirectorName(target);

            return guessDir == targetDir
                ? FeedbackColor.Green 
                : FeedbackColor.Red;
        }

        private string GetActorsText(DetailsSuperlistModel movie)
        {
            var actors = new List<string>();
            foreach (var actor in movie.Credits.Cast)
            {
                if (actor.Acting == "Acting")
                {
                    if (actors.Count >= 3)
                    {
                        continue;
                    }
                    
                    actors.Add(actor.ActorName);
                }
            } 
            
            return actors.Count == 0
                ? "N/A"
                : string.Join("\n ", actors);
        }

        private FeedbackColor GetActorsColor(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessActorIds = GetActorIds(guess);
            var targetActorIds = GetActorIds(target);

            if (guessActorIds.SetEquals(targetActorIds))
            {
                return FeedbackColor.Green;
            }

            if (guessActorIds.Overlaps(targetActorIds))
            {
                return FeedbackColor.Orange;
            }
            
            return FeedbackColor.Red;
        }

        private HashSet<int> GetActorIds(DetailsSuperlistModel movie)
        {
            var actorSet  = new HashSet<int>();
            if (movie.Credits.Cast == null)
            {
                return actorSet;
            }
            
            foreach (var actor in movie.Credits.Cast)
            {
                if (actor.Acting == "Acting")
                {
                    if (actorSet.Count >= 3)
                    {
                        break;
                    }

                    actorSet.Add(actor.ActorId);
                }
            }
            return actorSet;
        }

        private string GetGenresText(DetailsSuperlistModel movie)
        {
            var genres = new List<string>();
            foreach (var genre in movie.Genres)
            {
                genres.Add(genre.Name);
            }

            return genres.Count == 0 
                ? "N/A" 
                : string.Join("\n", genres);
        }

        private FeedbackColor GetGenresColor(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessGenreId = GetGenreIds(guess);
            var targetGenreId = GetGenreIds(target);

            if (guessGenreId.SetEquals(targetGenreId))
            {
                return FeedbackColor.Green;
            }

            if (guessGenreId.Overlaps(targetGenreId))
            {
                return FeedbackColor.Orange;
            }
            
            return FeedbackColor.Red;
        }

        private HashSet<int> GetGenreIds(DetailsSuperlistModel movie)
        {
            var genresSet = new HashSet<int>();
            
            if (movie.Genres == null)
            {
                return genresSet;
            }

            foreach (var genre in movie.Genres)
            {
                genresSet.Add(genre.Id);
            }
            
            return genresSet;
        }

        private string GetYearText(DetailsSuperlistModel movie)
        {
            const int lengthYearToRead = 4;
            var yearText = movie.Release_Date;

            return yearText[..lengthYearToRead];
        }

        private FeedbackColor GetYearColor(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessYear = GetYearText(guess);
            var targetYear = GetYearText(target);
            
            return guessYear == targetYear 
                ? FeedbackColor.Green 
                : FeedbackColor.Red;
        }

        private bool CompareYearsToDetermineArrowRot(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessYear = int.Parse(GetYearText(guess));
            var targetYear = int.Parse(GetYearText(target));

            return guessYear < targetYear;
        }
    }
}