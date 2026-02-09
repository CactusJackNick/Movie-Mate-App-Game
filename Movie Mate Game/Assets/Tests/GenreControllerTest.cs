using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Genre;
using DefaultNamespace.Models;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class GenreControllerTest
{
    private IGenreView _view;
    private IApiService _apiService;
    private GenreIconsConfig _config;
    private ILocalizationService _localizationService;
    
    private bool _closeRequestedCalled;
    
    [SetUp]
    public void SetUp()
    {
        _view = Substitute.For<IGenreView>(); 
        _apiService = Substitute.For<IApiService>();
        _config = ScriptableObject.CreateInstance<GenreIconsConfig>();
        _localizationService = Substitute.For<ILocalizationService>();
    }

    [TearDown]
    public void TearDown()
    {
        _view = null;
        _apiService = null;
        _localizationService = null;
        
        if (_config != null)
        {
            Object.DestroyImmediate(_config);
        }
    }
    
    [Test]
    public void GenreController_Configure_NoErrors()
    {
        // Arrange
            
        // Act
        var action = new TestDelegate
        (() =>
        {
            var genreController = new GenreController(_view, _apiService,_config, _localizationService);
        });
            
        // Assert
        Assert.DoesNotThrow(action);
    }
    
    [Test]
    public async Task LoadGenres_ApiReturnsItems_DisplaysCorrectViewModels() 
    {
        // Arrange
        var sut = new GenreController(_view,  _apiService, _config, _localizationService);
        _localizationService.GetCurrentLanguageCode().Returns("en");
        
        var mockResponse = new GenresListResponse 
        { 
            Genres = new List<GenreEntryDto>
            {
                new()
                {
                    Id = 101,
                    Name = "Action"
                },
                new()
                {
                    Id = 102,
                    Name = "Comedy"
                }
            }
        };
        
        _apiService.GetGenreListAsync().Returns(UniTask.FromResult(mockResponse));

        // Act
        await sut.LoadGenresAsync();
        
        // Assert
        _view.Received(1).DisplayGenres(Arg.Is<List<GenreViewModel>>
        (
            list =>  list.Count == 2 && 
                  list[0].Id == 101 &&
                  list[1].Name == "Comedy"
        ));
    }


    [Test]
    public void OnGenreClicked_RelativeGenre_ShowsMoviesFromGenre()
    {
        // Arrange
        const int eventID = 101;
        var controllerId = 0;
        var sut = new GenreController(_view, _apiService, _config, _localizationService);
        
        // Act
        sut.OnGenreSelected += HandleGenreRequested;
        _view.OnGenreClicked += Raise.Event<Action<int>>(eventID);
        
        // Assert
        Assert.AreEqual(controllerId, eventID);
        return;
        
        void HandleGenreRequested(int genreId)
        {
            controllerId = genreId;
        }
    }
    
    [Test]
    public void OnBackClicked_RaisesOnCloseRequested()
    {
        // Arrange
        var sut = new GenreController(_view, _apiService, _config, _localizationService);
        _closeRequestedCalled =  false;
        
        // Act
        sut.OnCloseRequested += HandleCloseRequested;
        _view.OnBackClicked += Raise.Event<Action>();
        
        // Assert
        Assert.IsTrue(_closeRequestedCalled);
        return;
        
        void HandleCloseRequested()
        {
            _closeRequestedCalled =  true;
        }
    }
}
