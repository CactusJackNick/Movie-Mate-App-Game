using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            
            _panel.gameObject.SetActive(false);
            _panel.rectTransform.localScale = Vector3.zero;
        }

        public void ShowClue(ClueData data)
        {
            _panel.rectTransform.DOKill();
            _title.text = data._title;
            
            DeactivateAllContainers();
        
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
            
            OpenPanelAnimationAsync().Forget();
        }
        
        public async UniTask OpenPanelAnimationAsync()
        {
            _panel.gameObject.SetActive(true);
            _panel.rectTransform.localScale = Vector3.zero;
            
            await _panel.rectTransform
                .DOScale(1f, 0.5f)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
        
        private void Hide()
        {
            HideClueAsync().Forget();
        }
        
        private async UniTask HideClueAsync()
        {
            _panel.rectTransform.DOKill();
            
            await _panel.rectTransform
                .DOScale(0f, 0.5f)
                .SetEase(Ease.OutExpo)
                .AsyncWaitForCompletion()
                .AsUniTask();
            
            _panel.gameObject.SetActive(false);
        }
        
        private void DeactivateAllContainers()
        {
            _textContainer.gameObject.SetActive(false);
            _portraitContainer.gameObject.SetActive(false);
            _backdropContainer.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_panel != null)
            {
                _panel.rectTransform.DOKill();
            }
            
            _closeButton.onClick.RemoveListener(Hide);
        }
    }
}