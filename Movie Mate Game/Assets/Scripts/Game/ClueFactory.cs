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
            
            var randomSprite = GetRandomSprite(backdrop);
            
            var clues = new List<ClueData>
            {
                new() //clue 1
                {
                    _title = "Backdrop",
                    _displayMode = ClueDisplayMode.Backdrop,
                    _imageContext = randomSprite
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

        private Sprite GetRandomSprite(Sprite original)
        {
            var randomMode = Random.Range(0, 3);

            switch (randomMode)
            {
                case 0:
                    return GetBlurredSprite(original);
                case 1:
                    return GetZoomedInSprite(original);
                default:
                    return original;
            }
        }

        private Sprite GetBlurredSprite(Sprite original)
        {
            const int downscaleAmount = 20;
            const int minimumSize = 8;
            var smallWidth = (int) Mathf.Max(minimumSize, original.rect.width / downscaleAmount);
            var smallHeight = (int) Mathf.Max(minimumSize, original.rect.height / downscaleAmount);
            
            var rt = RenderTexture.GetTemporary(smallWidth, smallHeight);
            rt.filterMode = FilterMode.Bilinear;
            
            Graphics.Blit(original.texture, rt);
            RenderTexture.active = rt;
            
            var blurredTex = new Texture2D(smallWidth, smallHeight);
            blurredTex.ReadPixels(new Rect(0, 0, smallWidth, smallHeight), 0, 0);
            blurredTex.Apply();
            
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            
            return Sprite.Create(blurredTex, new Rect(0, 0, smallWidth, smallHeight), new Vector2(0.5f, 0.5f));
        }

        private Sprite GetZoomedInSprite(Sprite original)
        {
            const float zoomFactor = 0.4f;
            var zoomedInWidth = original.rect.width * zoomFactor;
            var zoomedInHeight = original.rect.height * zoomFactor;
            
            var randomX = Random.Range(0, original.rect.width -  zoomedInWidth);
            var randomY = Random.Range(0, original.rect.height -  zoomedInHeight);
            
            var zoomRect = new Rect(randomX, randomY, zoomedInWidth, zoomedInHeight);
            
            return Sprite.Create(original.texture, zoomRect,  new Vector2(0.5f, 0.5f));
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