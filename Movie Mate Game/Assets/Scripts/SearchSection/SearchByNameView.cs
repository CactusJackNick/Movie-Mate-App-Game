using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SearchSection
{
    public class SearchByNameView : MonoBehaviour, ISearchByNameView
    { 
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_InputField _inputField;
        
        public event Action OnBackButtonPressed;
        public event Action<string> OnSearchButtonPressed;
        
        private void Awake()
        {
            _backButton.onClick.AddListener(GoBackToGenericSearch);
            _inputField.onValueChanged.AddListener(OnSubmit);
        }

        public void SetupViewInitialState()
        {
            _inputField.text = "";
        }
        
        private void GoBackToGenericSearch()
        {
            OnBackButtonPressed?.Invoke();
            _inputField.text = "";
        }

        private void OnSubmit(string text)
        {
            OnSearchButtonPressed?.Invoke(text);
        }
        
        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(GoBackToGenericSearch);
            _inputField.onValueChanged.RemoveListener(OnSubmit);
        }
    }
}