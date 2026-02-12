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
        [SerializeField] private Sprite _placeholderIcon;

        [Header("Navigation")]
        [SerializeField] private Button _button;

        public event Action backButtonPressed;
        private void Awake()
        {
            _button.onClick.AddListener(InvokeBackButtonPressed);
        }

        public void DisplayData(DetailsSuperlistModel data, string director, string genres)
        {
            if (_posterImage != null)
            {
                _posterImage.sprite = _placeholderIcon;
            }
            
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
            if (_posterImage == null)
            {
                return;
            }
            
            if (_posterImage != null)
            {
                _posterImage.sprite = sprite;
            }
        }
        
        public void ClearView()
        {
            _titleText.text = "";
            _descriptionText.text = "";
            _ratingText.text = "";
            _directorText.text = "";
            _releaseDateText.text = "";
            _taglineText.text = "";
            _genresText.text = "";
    
            if (_posterImage != null)
            {
                _posterImage.sprite = _placeholderIcon;
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