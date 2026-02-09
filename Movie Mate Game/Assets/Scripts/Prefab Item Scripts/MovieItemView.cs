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

        private void Awake()
        {
            _button.onClick.AddListener(OpenOnClick);
        }

        public void Setup(MovieData data)
        {
            _data = data;
            _title.text = data.Title;
            _overview.text = data.Overview;
            
            GetPosterAsync(data.poster_path).Forget();
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