using System;
using DefaultNamespace.OptionsSelector;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsView : MonoBehaviour, ISettingsView
    {
        [Header("Navigation")]
        [SerializeField] private Button _backButton;
        
        public event Action OnBackClicked;
        
        private void Awake()
        {
            _backButton.onClick.AddListener(BackToMain);
        }
        
        private void BackToMain()
        {
            OnBackClicked?.Invoke();
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(BackToMain);
        }
    }
}