using Mediator;
using UnityEngine;

namespace DefaultNamespace
{
    public class UIBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameObject _globalInputBlocker;

        private void Awake()
        {
            UINavigationMediator.Instance.SetGlobalBlocker(_globalInputBlocker);
            _globalInputBlocker.SetActive(false);
        }
    }
}