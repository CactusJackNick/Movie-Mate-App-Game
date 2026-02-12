using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Game
{
    public class GameView : MonoBehaviour, IGameView
    {
        [Header("InputField")]
        [SerializeField] private TMP_InputField _inputField;
        
        [Header("Navigation")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _debug;
        [SerializeField] private Button _clearSearchButton;
        
        [Header("List Settings")]
        [SerializeField] private Transform _contentParent;
        [SerializeField] private GuessItemButton _guessItemPrefab;
        
        [Header("Clue System")]
        [SerializeField] private Transform _contentClueParent;
        [SerializeField] private ClueButton _cluePrefab;
        [SerializeField] private CluePopup _cluePopup;
        
        [Header("Feedback System")]
        [SerializeField] private Transform _contentFeedbackParent;
        [SerializeField] private FeedbackGuessItem _feedbackGuessPrefab;
        [SerializeField] private ScrollRect _feedbackScrollRect;
        
        [SerializeField] private LeaveGamePopup _leaveGamePopup;
        
        [Header("Mobile UX")]
        [SerializeField] private RectTransform _movingContainer;
        
        private readonly List<GuessItemButton> _guessItems = new();
        private readonly List<ClueButton> _spawnedButtons = new();
        
        private readonly float _shiftY = 20f;
        private readonly float _animationDuration = 0.25f;
        private Vector2 _initialPos;
        private bool _isKeyboardActive;
        private int _debugCurrentClueIndex = 0;
        
        public event Action OnBackButtonPressed;
        public event Action OnDebugPressed;
        public event Action OnClearTextPressed;
        public event Action<string> OnInputPressed;
        public event Action<int> OnGuessSelected;
        
        private void Awake()
        {
            _backButton.onClick.AddListener(GoBackToMain);
            _debug.onClick.AddListener(DebugUnlockNext);
            _clearSearchButton.onClick.AddListener(OnClearSearch);
            _inputField.onValueChanged.AddListener(OnInputChanged);

            _leaveGamePopup.OnConfirmLeave += HandleConfirmLeaveGame;
            
            _inputField.text = string.Empty;
            _initialPos = _movingContainer.anchoredPosition;
        }

        private void Update()
        {
            if (TouchScreenKeyboard.visible && !_isKeyboardActive)
            { 
                OnKeyboardOpen();
            }
            else if (!TouchScreenKeyboard.visible && _isKeyboardActive)
            {
                OnKeyboardClose();
            }
        }

        public void DisplayMovies(List<MovieData> movies)
        {
            ClearGuessesItems();
            AddMovies(movies);
        }
        
        public void AddMovies(List<MovieData> movies)
        {
            foreach (var movieData in movies)
            {
                if (string.IsNullOrEmpty(movieData.Title) ||
                    string.IsNullOrEmpty(movieData.Poster_path)) //TODO: filter characters method
                {
                    continue;
                }
                
                var item = Instantiate(_guessItemPrefab, _contentParent);
                item.Setup(movieData);
                _guessItems.Add(item);
                item.OnClick += SubmitPlayerGuess;
            }
        }
        
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

        public void ShowFeedbackResult(GuessResultModel result)
        {
            var row = Instantiate(_feedbackGuessPrefab,  _contentFeedbackParent);
            row.Setup(result);
            
            _feedbackScrollRect.verticalNormalizedPosition = 0f;
        }
        
        public void ClearFeedbackItems()
        {
            foreach (Transform child in _contentFeedbackParent)
            {
                Destroy(child.gameObject);
            }
            
            _feedbackScrollRect.verticalNormalizedPosition = 1f;
        }

        public void ClearGuessesItems()
        {
            foreach (var item in _guessItems)
            {
                Destroy(item.gameObject);
            }
            
            _guessItems.Clear();
        }

        public void SetInputStatus(bool isEnabled)
        {
            _inputField.interactable = isEnabled;
        }

        public void UnlockClue(int index)
        {
            if (index >= 0 && index < _spawnedButtons.Count)
            {
                _spawnedButtons[index].Unlocked();
            }
        }
        
        private void SubmitPlayerGuess(MovieData obj)
        {
            Debug.Log($"Submitting player guess: {obj.Title}");
            
            ClearGuessesItems();

            _inputField.SetTextWithoutNotify(string.Empty);
            
            _clearSearchButton.gameObject.SetActive(false);
            
            OnGuessSelected?.Invoke(obj.Id);
        }

        private void OnClearSearch()
        {
            _inputField.SetTextWithoutNotify(string.Empty);
            _clearSearchButton.gameObject.SetActive(false);
            ClearGuessesItems();
            _inputField.Select();
            
            OnClearTextPressed?.Invoke();
        }

        private void OnInputChanged(string input)
        {
            _clearSearchButton.gameObject.SetActive(!string.IsNullOrEmpty(input));

            OnInputPressed?.Invoke(input);
        }
        
        private void GoBackToMain()
        {
            _leaveGamePopup.OpenPanelAnimationAsync().Forget();
        }

        private void HandleConfirmLeaveGame()
        {
            OnBackButtonPressed?.Invoke();
        }

        private void DebugUnlockNext()
        {
            _debugCurrentClueIndex++;
            if (_debugCurrentClueIndex < _spawnedButtons.Count)
            {
                OnDebugPressed?.Invoke();
                UnlockClue(_debugCurrentClueIndex);
            }
        }

        private void OnKeyboardOpen()
        {
            _movingContainer.DOKill();
            _isKeyboardActive = true;
            _movingContainer.DOAnchorPos(new Vector2(_initialPos.x, _initialPos.y + _shiftY), _animationDuration);
        }

        private void OnKeyboardClose()
        {
            _movingContainer.DOKill();
            _isKeyboardActive = false;
            _movingContainer.DOAnchorPos(_initialPos, _animationDuration);
        }
        
        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(GoBackToMain);
            _debug.onClick.RemoveListener(DebugUnlockNext);
            _clearSearchButton.onClick.RemoveListener(OnClearSearch);
            _inputField.onValueChanged.RemoveListener(OnInputChanged);
        }
    }
}