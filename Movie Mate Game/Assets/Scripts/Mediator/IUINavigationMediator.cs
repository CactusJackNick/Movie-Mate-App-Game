using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

namespace Mediator
{
    public interface IUINavigationMediator
    {
        void SetGlobalBlocker(GameObject blocker);
        void InjectToMediator(ABaseUIMediatorComponent panel);
        UniTask ReplacePanel(Panels first, Panels second);
    }
}