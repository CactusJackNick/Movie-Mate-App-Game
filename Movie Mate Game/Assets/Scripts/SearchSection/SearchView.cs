using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.SearchSection
{
    public class SearchView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _searchButton;
        [SerializeField] private Button _genreButton;
        
        public event Action OnBackClicked;
        public event Action OnFindMoviesClicked;
        public event Action OnGenreClicked;
        
        private void Awake()
        {
            _backButton.onClick.AddListener(BackToMain);
            _searchButton.onClick.AddListener(ToSearchScreen);
            _genreButton.onClick.AddListener(ToGenreScreen);
        }
        
        private void BackToMain()
        {
            OnBackClicked?.Invoke();
        }

        private void ToSearchScreen()
        {
            OnFindMoviesClicked?.Invoke();
        }

        private void ToGenreScreen()
        {
            OnGenreClicked?.Invoke();
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(BackToMain);
            _searchButton.onClick.RemoveListener(ToSearchScreen);
            _genreButton.onClick.RemoveListener(ToGenreScreen);
        }
    }
}