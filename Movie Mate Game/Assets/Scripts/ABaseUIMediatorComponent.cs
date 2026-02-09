using Mediator;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class ABaseUIMediatorComponent : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _backgroundCanvasGroup;
        [SerializeField] private CanvasGroup _panelCanvasGroup;
        [SerializeField] protected Panels _panelId;
        [SerializeField] private bool _hideOnAwake;
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
           if(_backgroundCanvasGroup.alpha < 1 ||
              _panelCanvasGroup.alpha < 1)
           {
               _backgroundCanvasGroup.alpha = 1; 
               _panelCanvasGroup.alpha = 1;
           }
            
           gameObject.SetActive(true);
        }
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}