namespace DefaultNamespace
{
    public class LoadingPanel : ABaseUIMediatorComponent, ILoadingPanel
    {
        private static LoadingPanel _instance;

        private LoadingPanel()
        {
            _instance = this;
        }
        
        public static LoadingPanel Instance => _instance;
    }
}