using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.OptionsSelector
{
    public class ResolutionSelector : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private OptionsButton _buttonPrefab;
        [SerializeField] private RectTransform _optionsHolder;
        [SerializeField] private string[] _resolutions;
        
        [Header("Colors")]
        [SerializeField] private Color _goldColor = new Color32(212, 178, 0, 255);
        [SerializeField] private Color _inactiveBgColor = Color.black;
        [SerializeField] private Color _activeTextColor = Color.white;
        
        private readonly Dictionary<int, OptionsButton> _buttons = new();
        
        public event Action<int> OnResolutionChanged; 

        public void SetupView()
        { 
           for(var i = 0; i < _resolutions.Length; i++)
           {
               if (_buttons.ContainsKey(i))
               {
                   return;
               }
               
               var optionButton = Instantiate(_buttonPrefab, _optionsHolder);
               optionButton.Setup
               (
                   idx: i,
                   text: _resolutions[i],
                   activeBg: _goldColor,
                   inactiveBg: _inactiveBgColor,
                   actText: _activeTextColor,
                   inaText: _goldColor
               );

               optionButton.OnClick += HandleClick;
               _buttons.Add(i, optionButton);
           }
        }
        
        public void SelectButtonAtIndex(int index)
        {
            if (index >= _resolutions.Length)
            {
                index = 0;
            }
        
            if (_buttons.TryGetValue(index, out var button))
            {
                button.InvokeOnClick();
            }
        }

        private void HandleClick(OptionsButton button)
        {              
            DisableButtons();
            button.SetActiveState(true);
            
            OnResolutionChanged?.Invoke(button.Index);
        }

        private void DisableButtons()
        {
            foreach (var button in _buttons)
            {
                button.Value.SetActiveState(false);
            }
        }
    }
}