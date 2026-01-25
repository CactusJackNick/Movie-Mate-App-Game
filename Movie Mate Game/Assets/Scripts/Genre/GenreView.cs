using System;
using System.Collections.Generic;
using DefaultNamespace.Models;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Genre
{
    public class GenreView : MonoBehaviour,  IGenreView
    {
        [Header("Navigation")]
        [SerializeField] private Button _backButton;

        [Header("Layout Settings")]
        [SerializeField] private Transform _contentParent; // Assign the Grid Layout Group here
        [SerializeField] private GenreButtonItem _itemPrefab; 

        private readonly List<GenreButtonItem> _spawnedItems = new();

        public event Action<int> OnGenreClicked;
        public event Action OnBackClicked;

        private void Awake()
        {
            _backButton.onClick.AddListener(BackToMain);
        }
        
        public void BackToMain()
        {
            OnBackClicked?.Invoke();
        }

        public void DisplayGenres(List<GenreViewModel> genres)
        {
            foreach (var item in _spawnedItems)
            {
                Destroy(item.gameObject);
            }
            _spawnedItems.Clear();

            foreach (var genreData in genres)
            {
                var newItem = Instantiate(_itemPrefab, _contentParent);
                newItem.Setup(genreData);
                
                _spawnedItems.Add(newItem);
            }
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(BackToMain);
        }
    }
}