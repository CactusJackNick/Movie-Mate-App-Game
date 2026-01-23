using DefaultNamespace;

namespace Mediator
{
    public interface IUINavigationMediator
    {
        void ShowPanel(Panels panelToOpen);
        void InjectToMediator(ABaseUIMediatorComponent panel);
        void ReplacePanel(Panels first, Panels second);
    }
}