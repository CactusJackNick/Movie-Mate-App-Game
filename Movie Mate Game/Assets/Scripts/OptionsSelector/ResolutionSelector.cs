using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.OptionsSelector
{
    public class ResolutionSelector : MonoBehaviour
    {
        private const string PrefResolutionIndex = "User_Resolution_Index";
        
        [Header("Buttons")]
        [SerializeField] private OptionsButton _buttonPrefab;
        [SerializeField] private RectTransform _optionsHolder;
        [SerializeField] private string[] _resolutions;
        
        [Header("Colors")]
        [SerializeField] private Color _goldColor = new Color32(212, 178, 0, 255);
        [SerializeField] private Color _inactiveBgColor = Color.black;
        [SerializeField] private Color _activeTextColor = Color.white;
        
        private readonly Dictionary<int, OptionsButton> _buttons = new();

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

        public void InitializeSavedState()
        {
            var savedIndex = PlayerPrefs.GetInt(PrefResolutionIndex);

            if (savedIndex > _resolutions.Length)
            {
                savedIndex = 0;
            }

            if (_buttons.TryGetValue(savedIndex, out var button))
            {
                button.InvokeOnClick();
            }
        }

        private void HandleClick(OptionsButton button)
        {              
            DisableButtons();
            button.SetActiveState(true);
            ApplyResolutionSettings(button.Index);
        }

        private void DisableButtons()
        {
            foreach (var button in _buttons)
            {
                button.Value.SetActiveState(false);
            }
        }
        
        private void ApplyResolutionSettings(int index)
        {
            var resolutionSize = "w342";
            switch (index)
            {
                case 0:
                    // low
                    resolutionSize = "w154";
                    break;
                case 1:
                    // mid
                    resolutionSize = "w342";
                    break;
                case 2:
                    //high
                    resolutionSize = "original";
                    break;
            }
            
            PlayerPrefs.SetInt(PrefResolutionIndex, index);
            PlayerPrefs.Save();
            
            ApiService.Instance.SetImageResolution(resolutionSize);
            Debug.Log($"Applying resolution {resolutionSize}");
        }
    }
}