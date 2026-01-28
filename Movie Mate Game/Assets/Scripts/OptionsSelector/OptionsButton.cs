using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.OptionsSelector
{
    public class OptionsButton : MonoBehaviour
    {
        [Header("Buttons Setup")]
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _buttonText;

        private int index;
        private Color _activeBgColor;
        private Color _inactiveBgColor;
        private Color _activeTextColor;
        private Color _inactiveTextColor;

        public event Action<OptionsButton> OnClick;
        public int Index => index;

        private void Awake()
        {
            _button.onClick.AddListener(InvokeOnClick);
        }

        public void Setup(int idx, string text, Color activeBg,
            Color inactiveBg, Color actText, Color inaText)
        {
            index = idx;
            _buttonText.text = text;
            _activeBgColor = activeBg;
            _inactiveBgColor = inactiveBg;
            _activeTextColor = actText;
            _inactiveTextColor = inaText;
        }

        public void SetActiveState(bool isActive)
        {
            _image.color = isActive 
                ? _activeBgColor 
                : _inactiveBgColor;
            
            _buttonText.color = isActive 
                ? _activeTextColor 
                : _inactiveTextColor;
        }
        
        public void InvokeOnClick()
        {
            OnClick?.Invoke(this); 
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(InvokeOnClick);
        }
    }
}