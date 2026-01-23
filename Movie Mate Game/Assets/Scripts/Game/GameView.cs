using System;
using System.Collections.Generic;
using DefaultNamespace.Models;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Game
{
    public class GameView : MonoBehaviour, IGameView
    {
        [SerializeField] private Button _backButton;
        
        [Header("List Settings")]
        [SerializeField] private Transform _contentParent;
        [SerializeField] private GameItemView _itemPrefab;
        
        private readonly List<GameItemView> _items = new();
        
        public event Action OnBackButtonPressed;
        private void Awake()
        {
            _backButton.onClick.AddListener(GoBackToMain);
        }

        public void GoBackToMain()
        {
            OnBackButtonPressed?.Invoke();
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
        
        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(GoBackToMain);
        }
    }
}