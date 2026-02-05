using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public class SearchController : ISearchController, IDisposable
    {
        private readonly IGameView _view;
        private readonly IApiService _apiService;
        
        private int _currentPage = 1;
        private int _maxPages = 1;
        private string _currentQuery = "";
        private bool _isLoading;
        private CancellationTokenSource _cts;
        private HashSet<int> _guessedIds;
        
        public SearchController(IGameView view, IApiService apiService)
        {
            _view = view;
            _apiService = apiService;
        }

        public void SetFilter(HashSet<int> guessedIds)
        {
            _guessedIds = guessedIds;
        }

        public void LoadNextPage()
        {
            if (_isLoading ||
                _currentPage >= _maxPages ||
                string.IsNullOrEmpty(_currentQuery))
            {
                return;
            }
            
            _currentPage++;
            LoadGuessMoviesAsync(_currentQuery, _currentPage).Forget();
        }
        
        public void StartSearch(string search)
        {
            ResetSearchState();
            _currentQuery = search;
            
            _cts = new CancellationTokenSource();
            SearchWithDebounce(search, _cts.Token).Forget();
        }

        public void ResetSearchState()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _currentQuery = "";
            _currentPage = 1;
            _maxPages = 1;
            _isLoading = false;
            _view.ClearGuessesItems();
        }

        private async UniTaskVoid SearchWithDebounce(string query, CancellationToken token)
        {
            await UniTask.Delay(500, cancellationToken: token);
            var page = 1;
            
            if (string.IsNullOrEmpty(query)) 
            {
                _view.ClearGuessesItems();
                return;
            }
            
            await LoadGuessMoviesAsync(query, page);
        }

        private async UniTask LoadGuessMoviesAsync(string query, int page)
        {
            _isLoading = true;
            try
            {
                var response = await _apiService.SearchMoviesAsync(query, page);
                if (_currentQuery != query)
                {
                    return;
                }

                _maxPages = response.TotalPages;
                var validMovies = FilterMovies(response.Results);

                if (page == 1)
                {
                    _view.DisplayMovies(validMovies);
                }
                else
                {
                    _view.AddMovies(validMovies);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private List<MovieData> FilterMovies(List<MovieData> input)
        {
            var list = new List<MovieData>();
            foreach (var data in input)
            {
                if (string.IsNullOrEmpty(data.backdrop_path) || string.IsNullOrEmpty(data.poster_path))
                {
                    continue;
                }
                
                if (_guessedIds != null && _guessedIds.Contains(data.Id))
                {
                    continue;
                }
                
                list.Add(data);
            }
            return list;
        }

        public void Dispose()
        {
            _cts?.Cancel();
        }
    }
}