using System;
using DefaultNamespace.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Genre
{
    public class GenreButtonItem : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Button _button;
        
        private int _myGenreId;
        public event Action<int> OnClick;
        
        public void Setup(GenreViewModel data)
        {
            _myGenreId =  data.Id;
            _nameText.text = data.Name.ToUpper();
            _iconImage.sprite = data.Icon;
            
            _button.onClick.AddListener(OpenGenreList);
        }

        private void OpenGenreList()
        {
            OnClick?.Invoke(_myGenreId);
            Debug.Log("Open Genre List");
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OpenGenreList);
        }
    }
}