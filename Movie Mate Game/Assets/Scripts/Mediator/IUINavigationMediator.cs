using DefaultNamespace;

namespace Mediator
{
    public interface IUINavigationMediator
    {
        void InjectToMediator(ABaseUIMediatorComponent panel);
        void ReplacePanel(Panels first, Panels second);
    }
}