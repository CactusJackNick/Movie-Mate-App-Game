using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Game
{
    public class ResultsView : MonoBehaviour, IResultsView
    {
        [Header("Parent Container")]
        [SerializeField] private RectTransform _resultsContainer;
        
        [Header("Result Images")]
        [SerializeField] private Image _trophyImage;
        
        [Header("Texts")]
        [SerializeField] private TMP_Text _resultsText;
        [SerializeField] private TMP_Text _movieGuessNameText;
        
        [Header("Buttons")]
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _exitButton;

        public event Action OnNewGameButtonPressed;
        public event Action OnExitButtonPressed;
        
        private void Awake()
        {
            _newGameButton.onClick.AddListener(OnNewGameStarted);
            _exitButton.onClick.AddListener(OnExitGame);
            
            _resultsContainer.localPosition = new Vector3(_resultsContainer.rect.width * 1.3f, 0, 0);
        }

        public async UniTask ShowResultsAsync(bool hasWon, string targetMovie)
        {
            SetupContent(hasWon, targetMovie);
            
            gameObject.SetActive(true);
            
            await _resultsContainer.transform
                .DOLocalMoveX(0, 3f)
                .SetEase(Ease.InBounce)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
        
        public void Hide()
        {
            HideResultsAsync().Forget();
        }

        private async UniTask HideResultsAsync()
        {
            await _resultsContainer.transform
                .DOLocalMoveX(_resultsContainer.rect.width * 1.3f, 1.5f)
                .SetEase(Ease.OutBounce)
                .AsyncWaitForCompletion()
                .AsUniTask();
            
            gameObject.SetActive(false);
        }
        

        public void SetupContent(bool hasWon, string movieGuessName)
        {
            _trophyImage.gameObject.SetActive(false);
            _movieGuessNameText.gameObject.SetActive(false);
            
            if (hasWon)
            {
                _resultsText.text = "Congratulations!";
                _trophyImage.gameObject.SetActive(true);
                _movieGuessNameText.text = string.Empty;
            }
            else
            {
                _resultsText.text = "Even the mightiest warrior may fall in the face of a forgotten title.\n Rest now; the IMDb gods will grant you another chance.";
                _movieGuessNameText.gameObject.SetActive(true);
                _movieGuessNameText.text = $"It was: '{movieGuessName}'";
            }
        }

        private void OnNewGameStarted()
        {
            OnNewGameButtonPressed?.Invoke();
        }

        private void OnExitGame()
        {
            OnExitButtonPressed?.Invoke();
        }

        private void OnDestroy()
        {
            _newGameButton.onClick.RemoveListener(OnNewGameStarted);
            _exitButton.onClick.RemoveListener(OnExitGame);
        }
    }
}