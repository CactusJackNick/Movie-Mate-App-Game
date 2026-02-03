using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ClueButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        
        private ClueData _clueData;
        private CluePopup _cluePopup;
        private bool _isUnlocked = false;
        
        private void Awake()
        {
            _button.onClick.AddListener(OpenCluePanel);
        }

        public void Setup(ClueData clue, CluePopup popup)
        {
            _clueData = clue;
            _cluePopup = popup;
            
            Locked();
        } 
        
        public void Unlocked()
        {
            //Play spin animation
            _isUnlocked = true;
            _button.interactable = true;
        }

        private void OpenCluePanel()
        {
            if (!_isUnlocked || _cluePopup == null)
            {
                return;
            }
            _cluePopup.OpenPanelAnimationAsync().Forget();
            _cluePopup.ShowClue(_clueData);
        }
        
        private void Locked()
        {
            _isUnlocked = false; 
            _button.interactable = false;  
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OpenCluePanel);
        }
    }
}