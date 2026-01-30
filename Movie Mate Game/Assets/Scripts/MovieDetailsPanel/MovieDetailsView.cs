using System;
using DefaultNamespace.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class MovieDetailsView : MonoBehaviour, IMovieDetailsView
    {
        [Header("Movie UI Elements")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _ratingText;
        [SerializeField] private TMP_Text _directorText;
        [SerializeField] private TMP_Text _releaseDateText;
        [SerializeField] private TMP_Text _taglineText;
        [SerializeField] private TMP_Text _genresText;
        [SerializeField] private Image _posterImage;

        [Header("Navigation")]
        [SerializeField] private Button _button;

        public event Action backButtonPressed;
        private void Awake()
        {
            _button.onClick.AddListener(InvokeBackButtonPressed);
        }

        public void DisplayData(DetailsSuperlistModel data, string director, string genres)
        {
            _titleText.text = data.Title;
            _descriptionText.text = data.Overview;
            _ratingText.text = data.Vote_Average.ToString("0.0");
            _releaseDateText.text = data.Release_Date;

            _directorText.text = director;
            _genresText.text = genres;
            
            _taglineText.text = !string.IsNullOrEmpty(data.Tagline)
                    ? data.Tagline
                    : "";
        }
        
        public void SetPoster(Sprite sprite)
        {
            if (_posterImage != null)
            {
                _posterImage.sprite = sprite;
            }
        }

        private void InvokeBackButtonPressed()
        {
            backButtonPressed?.Invoke();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(InvokeBackButtonPressed);
        }
    }
}