using System;

namespace DefaultNamespace.OptionsSelector
{
    public interface IResolutionSelector
    {
        event Action<int> OnResolutionChanged;

        void SetupView();
        void SelectButtonAtIndex(int index);
    }
}