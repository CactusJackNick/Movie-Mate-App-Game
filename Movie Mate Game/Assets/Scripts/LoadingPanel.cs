using UnityEngine;

namespace DefaultNamespace
{
    [DefaultExecutionOrder(-1000)]
    public class LoadingPanel : ABaseUIMediatorComponent, ILoadingPanel
    {
        private static LoadingPanel _instance;
        public static LoadingPanel Instance => _instance;
        
        public override void Awake()
        {
            base.Awake();

            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.LogWarning("Multiple LoadingPanel instances found. Using first instance.");
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}