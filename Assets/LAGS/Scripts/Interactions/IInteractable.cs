using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAGS
{
    public interface IInteractable
    {
        public void Interact(GameObject interactor);
        public void InteractExit(GameObject interactor);
    }
}
