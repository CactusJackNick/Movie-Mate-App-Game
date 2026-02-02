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
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _overview;
        [SerializeField] private Image _poster;
        [SerializeField] private Button _button;

        public event Action<MovieData> OnClick;
        private MovieData _data;

        public void Setup(MovieData data)
        {
            _data = data;
            _title.text = data.Title;
            _overview.text = data.Overview;
            
            GetPosterAsync(data.poster_path).Forget();
            InvokeThisButton();
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
        
        private void InvokeThisButton()
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OpenOnClick);
        }

        private void OpenOnClick()
        {
            OnClick?.Invoke(_data);
        }
    }
}