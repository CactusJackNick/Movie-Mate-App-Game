using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Mediator
{
    public class UINavigationMediator : IUINavigationMediator
    {
        private static IUINavigationMediator _instance;
        public static IUINavigationMediator Instance => _instance ??= new UINavigationMediator();
        
        private readonly Dictionary<Panels, ABaseUIMediatorComponent> _panelsDict = new();

        private UINavigationMediator()
        {
            _instance = this;
        }
        
        public void InjectToMediator(ABaseUIMediatorComponent panel)
        {
            if (!_panelsDict.TryAdd(panel.PanelId, panel))
            {
                Debug.LogWarning("PanelId " + panel.PanelId + " already exists in panels");
            }
        }
        
        public void ShowPanel(Panels panelId)
        {
            if (_panelsDict.TryGetValue(panelId, out var panel))
            {
                panel.Show();
            }
            else
            {
                Debug.LogWarning("Panel not found for Show: " + panelId);
            }
        }

        public void ReplacePanel(Panels first, Panels second)
        {
            HidePanel(first);
            ShowPanel(second);
        }

        public void HidePanel(Panels panelId)
        {
            if (_panelsDict.TryGetValue(panelId, out var panel))
            {
                panel.Hide();
            }
            else
            {
                Debug.LogWarning("Panel not found for Hide: " + panelId);
            } 
        }
        
    }
}