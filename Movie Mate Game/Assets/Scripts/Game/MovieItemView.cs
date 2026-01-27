using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Game
{
    public class MovieItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _overview;
        [SerializeField] private Image _poster;

        public void Setup(MovieData data)
        {
            _title.text = data.Title;
            _overview.text = data.Overview;

            GetPosterAsync(data.poster_path).Forget();
        }

        private async UniTaskVoid GetPosterAsync(string path)
        {
            var downloadedSprite = await ApiService.Instance.GetMovieImage(path);
            _poster.sprite = downloadedSprite; 
        }
    }
}