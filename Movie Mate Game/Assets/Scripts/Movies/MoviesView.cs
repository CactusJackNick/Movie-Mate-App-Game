using System;
using System.Collections.Generic;
using DefaultNamespace.Game;
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
        
        private void Awake()
        {
            _backButton.onClick.AddListener(GoBackToMain);
        }
        
        public void DisplayMovies(List<MovieData> movies)
        {
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }
            _items.Clear();

            foreach (var movieData in movies)
            {
                var item = Instantiate(_itemPrefab, _contentParent);
                item.Setup(movieData);
                _items.Add(item);
            }
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
    }
}