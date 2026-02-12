using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SearchSection
{
    public class SearchByNameView : MonoBehaviour, ISearchByNameView
    { 
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _clearTextButton;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Image _loadingSpinner;
        
        public event Action OnBackButtonPressed;
        public event Action<string> OnSearchButtonPressed;
        
        private void Awake()
        {
            _backButton.onClick.AddListener(GoBackToGenericSearch);
            _inputField.onValueChanged.AddListener(OnInputChanged);
            _clearTextButton.onClick.AddListener(OnClearText);
        }

        public void SetupViewInitialState()
        {
            _inputField.text = "";
            _clearTextButton.gameObject.SetActive(false);
        }

        public void SetLoadingSpinnerState(bool isActive)
        {
            _loadingSpinner.gameObject.SetActive(isActive);
        }
        
        private void GoBackToGenericSearch()
        {
            _inputField.text = "";
            OnBackButtonPressed?.Invoke();
        }

        private void OnInputChanged(string text)
        {
            _clearTextButton.gameObject.SetActive(!string.IsNullOrEmpty(text));
            OnSearchButtonPressed?.Invoke(text);
        }

        private void OnClearText()
        {
            _inputField.text = "";
            _inputField.Select();
        }
        
        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(GoBackToGenericSearch);
            _inputField.onValueChanged.RemoveListener(OnInputChanged);
            _clearTextButton.onClick.RemoveListener(OnClearText);
        }
    }
}