using System;
using System.Collections.Generic;
using DefaultNamespace.Models;
using TMPro;
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
        [SerializeField] private TMP_Text _noMoviesText;
        
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
                if (!IsSafeFontString(movieData.Title))
                {
                    continue;
                }
                
                var item = Instantiate(_itemPrefab, _contentParent);
                item.Setup(movieData);
                _items.Add(item);
                item.OnClick += OpenMovieDetailsPanel;
            }
        }

        public void ClearItems()
        {
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }
            
            _items.Clear();
        }

        public void ShowNoMoviesText(bool isActive)
        {
            _noMoviesText.gameObject.SetActive(isActive);
        }
        
        private void GoBackToMain()
        {
            OnBackButtonPressed?.Invoke();
        }
        
        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(GoBackToMain);
        }
        
        private bool IsSafeFontString(string text)
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
            Debug.Log("OpenMovieDetailsPanel");
        }
    }
}