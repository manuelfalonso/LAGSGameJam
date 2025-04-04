using UnityEngine;

namespace LAGS
{
    public interface IDraggable
    {
        public void Drag(GameObject interactor);

        public void Drop(GameObject interactor);
    }
}
