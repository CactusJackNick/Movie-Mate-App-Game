using DefaultNamespace.Models;
using TMPro;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public class GameItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _overview;

        public void Setup(MovieData data)
        {
            _title.text = data.Title;
            _overview.text = data.Overview;
        }
        
    }
}