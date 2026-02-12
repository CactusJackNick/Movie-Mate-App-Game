using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GuessItemButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _poster;
        [SerializeField] private Button _button;

        private int _id;
        public event Action<MovieData> OnClick;
        private MovieData _data;

        public int ID => _id;

        private void Awake()
        {
            _button.onClick.AddListener(OnClicked);
        }

        public void Setup(MovieData data)
        {
            _data = data;
            _id = data.Id;
            _title.text = data.Title;
            
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

        private void OnClicked()
        {
            OnClick?.Invoke(_data);
            Debug.Log("Guess item button clicked");
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClicked);
        }
    }
}