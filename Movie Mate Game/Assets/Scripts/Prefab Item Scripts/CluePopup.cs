using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class CluePopup : MonoBehaviour
    {
        [Header("Main Window")]
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Image _panel;
        
        [Header("Parent Containers")]
        [SerializeField] private Image _textContainer;     
        [SerializeField] private Image _portraitContainer;
        [SerializeField] private Image _backdropContainer;
        
        [Header("UI Components")]
        [SerializeField] private TMP_Text _clueText;
        [SerializeField] private Image _portrait;
        [SerializeField] private Image _backdrop;
        
        private void Awake()
        {
            _closeButton.onClick.AddListener(Hide);
        }

        public void ShowClue(ClueData data)
        {
            _title.text = data._title;
            
            DeactivateAll();
        
            switch (data._displayMode)
            {
                case ClueDisplayMode.Text:
                    _textContainer.gameObject.SetActive(true);
                    _clueText.text = data._textContext;
                    break;
                case ClueDisplayMode.Poster:
                    _portraitContainer.gameObject.SetActive(true);
                    _portrait.sprite = data._imageContext;
                    break;
                case ClueDisplayMode.Backdrop:
                    _backdropContainer.gameObject.SetActive(true);
                    _backdrop.sprite = data._imageContext;
                    break;
            }
            
            _panel.gameObject.SetActive(true);
        }

        private void DeactivateAll()
        {
            _textContainer.gameObject.SetActive(false);
            _portraitContainer.gameObject.SetActive(false);
            _backdropContainer.gameObject.SetActive(false);
        }
        
        private void Hide()
        {
            _panel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(Hide);
        }
    }
}