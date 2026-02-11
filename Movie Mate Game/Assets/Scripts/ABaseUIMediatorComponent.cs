using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mediator;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class ABaseUIMediatorComponent : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] protected CanvasGroup _fadeGroup;
        [SerializeField] protected Panels _panelId;
        [SerializeField] private bool _hideOnAwake;
        
        [Header("Transition Settings")]
        [SerializeField] protected bool _useTransitions = true;
        [SerializeField] protected RectTransform _animationTarget;
        
        public Panels PanelId => _panelId;

        public virtual void Awake()
        {
            UINavigationMediator.Instance.InjectToMediator(this);
            if (_hideOnAwake)
            {
                Hide();
            }
        }

        public virtual void Show()
        {
            _animationTarget.DOKill(); 
            _fadeGroup.DOKill();
            
            if (_animationTarget != null)
            {
                _animationTarget.anchoredPosition = Vector2.zero;
            }
            
            gameObject.SetActive(true);
        
            if (_useTransitions && _fadeGroup != null)
            {
                _fadeGroup.alpha = 0f;
                OnPanelOpen().Forget();
            }
            else if (_fadeGroup != null)
            {
                _fadeGroup.alpha = 1f;
            }
        }
        
        public virtual void Hide()
        {
            CloseAsync().Forget();
        }

        public async UniTask CloseAsync()
        {
            _animationTarget.DOKill();
            _fadeGroup.DOKill();
            
            if (!_useTransitions || _animationTarget == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            await _animationTarget.DOAnchorPos(new Vector2(Screen.width, 0), 1f)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion()
                .AsUniTask();
        
            gameObject.SetActive(false);
            _animationTarget.anchoredPosition = Vector2.zero;
        }
        
        private async UniTask OnPanelOpen()
        {
            _fadeGroup.alpha = 0f;

            await _fadeGroup.DOFade(1f, 0.5f)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
    }
}