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
        
        [Header("Scroll References")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _contentParent;
        [SerializeField] private RectTransform _viewport;
        
        [Header("Movie Settings")]
        [SerializeField] private MovieItemView _itemPrefab;
        [SerializeField] private Image _loadingSpinner;
        
        [Header("Virtualization")]
        [SerializeField] private float _itemVisualHeight = 110f;
        [SerializeField] private float _spacing = 10f;
        [SerializeField] private int _bufferCount = 5;
        
        private float ItemStep => _itemVisualHeight + _spacing;
        
        private readonly List<MovieData> _allData = new();
        private readonly Dictionary<int, MovieItemView> _activeItems = new();
        private readonly Stack<MovieItemView> _pool = new();
        
        private int _previousStartIndex = -1;
        private int _previousEndIndex = -1;
        
        public event Action OnBackButtonPressed;
        public event Action<MovieData> DetailsButtonClicked;

        private void Awake()
        {
            _backButton.onClick.AddListener(GoBackToMain);
            _scrollRect.onValueChanged.AddListener(_ => UpdateVisibleItems());
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
                
                _allData.Add(movieData);
            }

            float totalHeight = _allData.Count * ItemStep;
            _contentParent.sizeDelta = new Vector2(_contentParent.sizeDelta.x, totalHeight);

            UpdateVisibleItems();
        }

        public void SetLoadingSpinnerState(bool isActive)
        {
            _loadingSpinner.gameObject.SetActive(isActive);
        }

        public void ClearItems()
        {
            foreach (var kvp in _activeItems)
            {
                kvp.Value.gameObject.SetActive(false);
                _pool.Push(kvp.Value);
            }
            
            _activeItems.Clear();
            _allData.Clear();
            _contentParent.sizeDelta = new Vector2(_contentParent.sizeDelta.x, 0);
            _scrollRect.verticalNormalizedPosition = 1f;
            _previousStartIndex = -1;
            _previousEndIndex = -1;
        }

        private void UpdateVisibleItems()
        {
            if (_allData.Count == 0) return;

            float contentY = _contentParent.anchoredPosition.y;

            int startIndex = Mathf.FloorToInt(contentY / ItemStep);
            int endIndex = Mathf.CeilToInt((contentY + _viewport.rect.height) / ItemStep);

            startIndex = Mathf.Max(0, startIndex - _bufferCount);
            endIndex = Mathf.Min(_allData.Count - 1, endIndex + _bufferCount);

            if (startIndex == _previousStartIndex && endIndex == _previousEndIndex) return;

            List<int> indexesToRemove = new List<int>();
            foreach (var kvp in _activeItems)
            {
                if (kvp.Key < startIndex || kvp.Key > endIndex)
                {
                    indexesToRemove.Add(kvp.Key);
                }
            }

            foreach (int i in indexesToRemove)
            {
                ReturnItem(i);
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (!_activeItems.ContainsKey(i))
                {
                    SpawnItem(i);
                }
            }

            _previousStartIndex = startIndex;
            _previousEndIndex = endIndex;
        }

        private void SpawnItem(int index)
        {
            MovieItemView item;

            if (_pool.Count > 0)
            {
                item = _pool.Pop();
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Instantiate(_itemPrefab, _contentParent);
                item.OnClick += OpenMovieDetailsPanel;
            }

            RectTransform rect = item.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(0, _itemVisualHeight);
            rect.anchoredPosition = new Vector2(0, -index * ItemStep);

            item.Setup(_allData[index]);
            _activeItems.Add(index, item);
        }

        private void ReturnItem(int index)
        {
            if (_activeItems.TryGetValue(index, out var item))
            {
                item.gameObject.SetActive(false);
                _pool.Push(item);
                _activeItems.Remove(index);
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
