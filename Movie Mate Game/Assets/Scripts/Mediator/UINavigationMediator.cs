using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

namespace Mediator
{
    public class UINavigationMediator : IUINavigationMediator
    {
        private static IUINavigationMediator _instance;
        
        private readonly Dictionary<Panels, ABaseUIMediatorComponent> _panelsDict = new();
        
        private GameObject _globalBlocker;

        private UINavigationMediator()
        {
            _instance = this;
        }
        
        public static IUINavigationMediator Instance => _instance ??= new UINavigationMediator();
        
        public void InjectToMediator(ABaseUIMediatorComponent panel)
        {
            if (!_panelsDict.TryAdd(panel.PanelId, panel))
            {
                Debug.LogWarning("PanelId " + panel.PanelId + " already exists in panels");
            }
        }

        public async UniTask ReplacePanel(Panels first, Panels second)
        {
            _globalBlocker.SetActive(true);
            
            if (_panelsDict.TryGetValue(second, out var nextPanel))
            {
                nextPanel.Show(); 
            }
            
            if (_panelsDict.TryGetValue(first, out var currentPanel))
            {
                await currentPanel.CloseAsync();
            }

            _globalBlocker.SetActive(false);
        }
        
        public void SetGlobalBlocker(GameObject blocker)
        {
            _globalBlocker = blocker;
        }
    }
}