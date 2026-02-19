using System;
using System.Collections.Generic;
using DefaultNamespace.Models;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class MoviesView : MonoBehaviour, IMovieView
    {
        [SerializeField] private Button _backButton;
        
        [Header("Movie Settings")]
        [SerializeField] private Transform _contentParent;
        [SerializeField] private MovieItemView _itemPrefab;
        [SerializeField] private Image _loadingSpinner;
        
        private readonly List<MovieItemView> _items = new();
        
        public event Action OnBackButtonPressed;
        public event Action<MovieData> DetailsButtonClicked;
        
        private void Awake()
        {
            _backButton.onClick.AddListener(GoBackToMain);
        }
        
        public void DisplayMovies(List<MovieData> movies)
        {
            ClearItems();
            AddMovies(movies);
        }
        
        public void AddMovies(List<MovieData> movies)
        {
            foreach (var movieData in movies)
            {
                if (!IsSafeLanguageFontString(movieData.Title))
                {
                    continue;
                }
                
                var item = Instantiate(_itemPrefab, _contentParent);
                item.Setup(movieData);
                _items.Add(item);
                item.OnClick += OpenMovieDetailsPanel;
            }
        }
        
        public void SetLoadingSpinnerState(bool isActive)
        {
            _loadingSpinner.gameObject.SetActive(isActive);
        }

        public void ClearItems()
        {
            foreach (var item in _items)
            {
                if (item == null)
                {
                    continue;
                }

                item.OnClick -= OpenMovieDetailsPanel;
                Destroy(item.gameObject);
            }
            
            _items.Clear();

            for (var i = _contentParent.childCount - 1; i >= 0; i--)
            {
                Destroy(_contentParent.GetChild(i).gameObject);
            }
        }
        
        private void GoBackToMain()
        {
            OnBackButtonPressed?.Invoke();
        }
        
        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(GoBackToMain);
        }
        
        private bool IsSafeLanguageFontString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            foreach (var c in text)
            { 
                var isLatin = (c <= 255);
        
                var isCyrillic = (c >= 0x0400 && c <= 0x04FF);

                if (!isLatin && !isCyrillic) 
                {
                    return false;
                }
            }
            return true;
        }

        private void OpenMovieDetailsPanel(MovieData movieData)
        {
            DetailsButtonClicked?.Invoke(movieData);
        }
    }
}