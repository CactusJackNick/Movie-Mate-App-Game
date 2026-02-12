using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class MovieItemView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _overview;
        [SerializeField] private Button _button;
        [SerializeField] private Image _poster;
        
        [Header("Star Rating")]
        [SerializeField] private RectTransform _starsContainer;
        [SerializeField] private RectTransform _ratingMask;

        private MovieData _data;
        
        public event Action<MovieData> OnClick;

        private void Awake()
        {
            _button.onClick.AddListener(OpenOnClick);
        }

        public void Setup(MovieData data)
        {
            _data = data;
            _title.text = data.Title;
            _overview.text = data.Overview;
            SetupStarsRating(data.Vote_average);
            GetPosterAsync(data.Poster_path).Forget();
        }

        private async UniTaskVoid GetPosterAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            var downloadedSprite = await ApiService.Instance.GetMovieImageAsync(path);
            
            if (this == null || transform == null) 
            {
                return; 
            }

            if (downloadedSprite != null)
            {
                _poster.sprite = downloadedSprite; 
            }
        }

        private void SetupStarsRating(float voteAverage)
        {
            var percentage = Mathf.Clamp01(voteAverage / 10f);
            
            var totalWidth = _starsContainer.rect.width;
            var newWidth = totalWidth * percentage;

            _ratingMask.sizeDelta = new Vector2(newWidth, _ratingMask.sizeDelta.y);
        }
        
        private void OpenOnClick()
        {
            OnClick?.Invoke(_data);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OpenOnClick);
        }
    }
}