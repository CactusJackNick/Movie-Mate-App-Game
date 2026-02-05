using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LeaveGamePopup : MonoBehaviour
    {
        [SerializeField] private Image _panel;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _noButton;
        [SerializeField] private Button _yesButton;
        
        public event Action OnConfirmLeave;

        private void Awake()
        {
            _closeButton.onClick.AddListener(Hide);
            _yesButton.onClick.AddListener(OnYesPressed);
            _noButton.onClick.AddListener(Hide);
            
            _panel.gameObject.SetActive(false);
            _panel.rectTransform.localScale = Vector3.zero;
        }
        
        private void OnYesPressed()
        {
            OnConfirmLeave?.Invoke();
            Hide();
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

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(Hide);
            _yesButton.onClick.RemoveListener(OnYesPressed);
            _noButton.onClick.RemoveListener(Hide);
        }
    }
}