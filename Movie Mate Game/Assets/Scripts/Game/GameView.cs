using System;
using System.Collections.Generic;
using DefaultNamespace.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Game
{
    public class GameView : MonoBehaviour, IGameView
    {
        
        [Header("Movie UI Elements")]
        [SerializeField] private TMP_Text _actorsText;
        [SerializeField] private TMP_Text _directorText;
        [SerializeField] private TMP_Text _releaseDateText;
        [SerializeField] private TMP_Text _taglineText;
        [SerializeField] private TMP_Text _genresText;
        [SerializeField] private Image _posterImage;
        [SerializeField] private Image _backdropImage;
        
        [Header("Navigation")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _debug;
        
        [Header("List Settings")]
        [SerializeField] private Transform _contentParent;
        [SerializeField] private MovieItemView _itemPrefab;
        
        [Header("Clue System")]
        [SerializeField] private Transform _contentClueParent;
        [SerializeField] private ClueButton _cluePrefab;
        [SerializeField] private CluePopup _cluePopup;
        
        private readonly List<MovieItemView> _items = new();
        private readonly List<ClueButton> _spawnedButtons = new();
        
        private int _debugCurrentClueIndex = 0;
        public event Action OnBackButtonPressed;
        public event Action OnDebugPressed;
        
        private void Awake()
        {
            _backButton.onClick.AddListener(GoBackToMain);
            _debug.onClick.AddListener(DebugUnlockNext);
        }

        // public void ShowTier1(Sprite backdrop)
        // {
        //     _backdropImage.sprite = backdrop;
        //     _backdropImage.gameObject.SetActive(true);
        // }
        
        public void GoBackToMain()
        {
            OnBackButtonPressed?.Invoke();
        }

        public void DebugUnlockNext()
        {
            _debugCurrentClueIndex++;
            if (_debugCurrentClueIndex < _spawnedButtons.Count)
            {
                OnDebugPressed?.Invoke();
                UnlockClue(_debugCurrentClueIndex);
            }
        }

        public void AssignData(DetailsSuperlistModel data, string director, string actors, string genres)
        {
            _directorText.text = director;
            _actorsText.text = actors;
            _genresText.text = genres;
            _releaseDateText.text = data.Release_Date;
            _taglineText.text = data.Tagline;
            SetPoster(_posterImage.sprite);
            SetBackdrop(_backdropImage.sprite);
            
        }
        
        public void SetPoster(Sprite poster)
        {
            if (_posterImage != null)
            {
                _posterImage.sprite = poster;
            }
        }

        public void SetBackdrop(Sprite backdrop)
        {
            if (_backdropImage != null)
            {
                _backdropImage.sprite = backdrop;
            }
        }
        
        // public void DisplayMovies(List<MovieData> movies) //TODO: fix for dropdown selection
        // {
        //     foreach (var item in _items)
        //     {
        //         Destroy(item.gameObject);
        //     }
        //     _items.Clear();
        //
        //     foreach (var movieData in movies)
        //     {
        //         var item = Instantiate(_itemPrefab, _contentParent);
        //         item.Setup(movieData);
        //         _items.Add(item);
        //     }
        // }

        public void DisplayClues(List<ClueData> clues)
        {
            _debugCurrentClueIndex = 0;
            foreach (var clueButton in _spawnedButtons)
            {
                Destroy(clueButton.gameObject);
            }
            _spawnedButtons.Clear();

            foreach (var data in clues)
            {
                var item = Instantiate(_cluePrefab, _contentClueParent);
                item.Setup(data, _cluePopup);
                _spawnedButtons.Add(item);
            }
        }
        
        public void UnlockClue(int index)
        {
            if (index >= 0 && index < _spawnedButtons.Count)
            {
                _spawnedButtons[index].Unlocked();
            }
        }
        
        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(GoBackToMain);
            _debug.onClick.RemoveListener(DebugUnlockNext);
        }
    }
}