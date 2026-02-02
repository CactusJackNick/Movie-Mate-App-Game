using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public class GameController : IGameController
    {
        private readonly IGameView _view;
        private readonly IApiService _apiService;

        private int _currentTier = 0;

        public GameController(IGameView view, IApiService apiService)
        {
            _view = view;
            _apiService = apiService;
        }

        public void LoadMovies()
        { 
            LoadMovieAsync().Forget();
            //LoadMoviesAsync().Forget();
        }

        private async UniTask LoadMovieAsync()
        {
            LoadingPanel.Instance.Show();
            
            var movie = await GetValidGameMovieAsync();
            if (movie == null)
            {
                return;
            }   
            var poster = await _apiService.GetMovieImageAsync(movie.PosterPath);
            var backdrop = await _apiService.GetMovieImageAsync(movie.BackdropPath);

            var date = movie.Release_Date;
            var tagline = movie.Tagline;
            
            var directorName = movie.Credits.Crew.Find(x => x.Job == "Director").NameCrew ?? "MISSING";
           
            var genresText = "";
            var genresList = new List<string>();
            foreach (var genre in movie.Genres)
            {
                genresList.Add(genre.Name);
            } 
            
            var actorsText = "";
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
                
            genresText = string.Join(", ", genresList);
            actorsText = string.Join(", ", actors);
            
            var clues = AssignDataToClues
            (
                director: directorName,
                date: date,
                actors: actorsText,
                genres: genresText, 
                tagline: tagline,
                backdrop: backdrop,
                poster: poster
            );
           
            _view.DisplayClues(clues);
            _view.UnlockClue(0);
            
            LoadingPanel.Instance.Hide();
        }

        private List<ClueData> AssignDataToClues(
            string director, string date, string actors, string genres, 
            string tagline, Sprite backdrop, Sprite poster)
        {
            var clues = new List<ClueData>();
            
            clues.Add(new ClueData //clue 1
            {
                _title = "Backdrop",
                _displayMode = ClueDisplayMode.Backdrop,
                _imageContext = backdrop
            });
            
            clues.Add(new ClueData //clue 2
            {
                _title = "Year & Genres",
                _displayMode = ClueDisplayMode.Text, 
                _textContext = $"{date}\n{genres}"
            });
            
            clues.Add(new ClueData //clue 3
            {
                _title = "Director",
                _displayMode = ClueDisplayMode.Text,
                _textContext = $"{director}"
            });
            
            clues.Add(new ClueData //clue 4
            {
                _title = "Actors",
                _displayMode = ClueDisplayMode.Text,
                _textContext = $"{actors}"
            });
            
            clues.Add(new ClueData //clue 5
            {
                _title = "Quote",
                _displayMode = ClueDisplayMode.Text,
                _textContext = $"{tagline}"
            });
            
            clues.Add(new ClueData //clue 6
            {
                _title = "Poster",
                _displayMode = ClueDisplayMode.Poster,
                _imageContext = poster
            });
            
            return clues;
        }

        // private async UniTask LoadMoviesAsync() //TODO: load movies to select answer
        // {
        //     try
        //     {
        //         var listResponse = await _apiService.GetPopularMoviesAsync(1); // need to update
        //
        //         if (listResponse.Results is { Count: > 0 })
        //         {
        //             _view.DisplayMovies(listResponse.Results);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.LogError($"Error loading movies: {ex.Message} ");
        //         throw new Exception(ex.Message);
        //     }
        // }

        // private void AdvanceTier(Sprite backdropSprite)
        // {
        //     _currentTier++;
        //
        //     switch (_currentTier)
        //     {
        //         case 0:
        //             _view.ShowTier1(backdropSprite);
        //             break;
        //     }
        // }
        
        private async UniTask<DetailsSuperlistModel> GetValidGameMovieAsync()
        {
            const int maxPageSize = 100;
            var maxAttempts = 10;
            var attempts = 0;

            while (attempts < maxAttempts)
            {
                attempts++;
                var randomPageNumber = Random.Range(1, maxPageSize);
                var listResponse = await _apiService.GetPopularMoviesAsync(randomPageNumber);

                if (listResponse.Results == null || listResponse.Results.Count == 0)
                {
                    continue;
                }
                
                var candidates = ShuffleListRandomly(listResponse.Results);

                foreach (var candidate in candidates)
                {
                    if (string.IsNullOrEmpty(candidate.backdrop_path))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(candidate.poster_path))
                    {
                        continue;
                    }

                    if (candidate.Genre_Ids == null || candidate.Genre_Ids.Count == 0)
                    {
                        continue;
                    }

                    var details = await _apiService.GetMovieDetailsAsync(candidate.Id);

                    if (ValidateMovie(details))
                    {
                        Debug.Log($"Found valid game movie: {details.Title}");
                        return details;
                    }
                }
            }

            return null;
        }

        private bool ValidateMovie(DetailsSuperlistModel movie)
        {
            if (string.IsNullOrEmpty(movie.Tagline))
            {
                return false;
            }

            var hasDirector = false;
            foreach (var person in movie.Credits.Crew)
            {
                if (person.Job == "Director")
                {
                    hasDirector = true;
                }
            }
           
            if (!hasDirector)
            {
                return false;
            }

            if (movie.Credits.Cast == null || movie.Credits.Cast.Count < 3)
            {
                return false;
            }
            
            return true;
        }

        private List<MovieData> ShuffleListRandomly(List<MovieData> inputList)
        {    
            //take any list of DetailsSuperlistModel and return it with Fischer-Yates shuffle
            var i = 0;
            var t = inputList.Count;
            MovieData p;
            var tempList = new List<MovieData>();
            tempList.AddRange(inputList);
     
            while (i < t)
            {
                var r = Random.Range(i, tempList.Count);
                p = tempList[i];
                tempList[i] = tempList[r];
                tempList[r] = p;
                i++;
            }
     
            return tempList;
        }
        public void Dispose()
        {
            //this.Dispose();
        }
    }
    
}