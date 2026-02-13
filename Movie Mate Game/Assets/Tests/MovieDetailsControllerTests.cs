using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Models;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class MovieDetailsControllerTests
    {
        private IMovieDetailsView _view;
        private IApiService _api;
        private ILoadingPanel _loadingPanel;

        [SetUp]
        public void Setup()
        {
            _view = Substitute.For<IMovieDetailsView>();
            _api = Substitute.For<IApiService>();
            _loadingPanel = Substitute.For<ILoadingPanel>();
        }

        [TearDown]
        public void Teardown()
        {
            _view = null;
            _api = null;
            _loadingPanel = null;
        }

        [Test]
        public async Task LoadMovieInfo_Success_PopulatesViewAndHidesLoading()
        {
            // Arrange
            var sut = new MovieDetailsController(_view, _api, _loadingPanel);
            var movieId = 123;
            var dummyDetails = new DetailsSuperlistModel
            {
                Title = "Test Movie",
                PosterPath = "path",
                Genres = new List<GenreEntryDto>
                {
                    new()
                    {
                        Name = "Action"
                    },
                    new()
                    {
                        Name = "Meta"
                    }
                },
                Credits = new Credits
                {
                    Crew = new List<CrewMember>
                    {
                        new()
                        {
                            Job = "Producer Director",
                            NameCrew = "Jeff"
                        },
                        new()
                        {
                            Job = "Director",
                            NameCrew = "Nolan"
                        }
                    }
                }
            };

            var dummySprite = Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero);

            _api.GetMovieDetailsAsync(movieId).Returns(UniTask.FromResult(dummyDetails));
            _api.GetMovieImageAsync(dummyDetails.PosterPath).Returns(UniTask.FromResult(dummySprite));

            // Act
            sut.LoadMovieInfo(movieId);
            await UniTask.Delay(100);

            // Assert
            _loadingPanel.Received(1).Show();
            _loadingPanel.Received(1).Hide();

            _view.Received(1).DisplayData(
                dummyDetails,
                "Nolan",
                "Action, Meta"
            );

            _ = _api.Received(1).GetMovieImageAsync("path");
            _view.Received(1).SetPoster(dummySprite);
        }
        
        [Test]
        public async Task LoadMovieInfo_NoDirectorOrGenres_DisplaysDefaults()
        {
            // Arrange
            var sut = new MovieDetailsController(_view, _api, _loadingPanel);
            var dummyDetails = new DetailsSuperlistModel
            {
                Credits = new Credits
                {
                    Crew = new List<CrewMember>()
                },
                Genres = new List<GenreEntryDto>() 
            };

            _api.GetMovieDetailsAsync(Arg.Any<int>()).Returns(UniTask.FromResult(dummyDetails));

            // Act
            sut.LoadMovieInfo(1);
            await Task.Delay(50);

            // Assert
            _view.Received(1).DisplayData(dummyDetails, "Unknown", "");
        }
    
        [Test]
        public async Task LoadMovieInfo_NullPosterPath_DoesNotCallImageApi()
        {
            // Arrange
            var sut = new MovieDetailsController(_view, _api, _loadingPanel);
            var dummyDetails = new DetailsSuperlistModel
            {
                PosterPath = null,
                Credits = new Credits(),
                Genres = new List<GenreEntryDto>()
            };

            _api.GetMovieDetailsAsync(1).Returns(UniTask.FromResult(dummyDetails));

            // Act
            sut.LoadMovieInfo(1);
            await Task.Delay(100);

            // Assert
            _ = _api.DidNotReceive().GetMovieImageAsync(Arg.Any<string>());
            _view.DidNotReceive().SetPoster(Arg.Any<Sprite>());
        }

        [Test]
        public void OnBackButtonPressed_TriggersControllerEvent()
        {
            // Arrange
            var sut = new MovieDetailsController(_view, _api, _loadingPanel);
            var wasCalled = false;
            sut.OnBackButtonRequested += HandleCloseRequested;

            // Act
            _view.backButtonPressed += Raise.Event<Action>();

            // Assert
            Assert.IsTrue(wasCalled);
            return;
            
            void HandleCloseRequested()
            {
                wasCalled =  true;
            }
        }
    }
}