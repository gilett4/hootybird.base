//@vadym udod

using ByteSheep.Events;
using UnityEngine;

namespace hootybird.UI.Helpers
{
    public class Selectable3D : MonoBehaviour
    {
        public AdvancedEvent onPointerEnter;
        public AdvancedEvent onPointerExit;

        public void OnEnter()
        {
            onPointerEnter.Invoke();
        }

        public void OnExit()
        {
            onPointerExit.Invoke();
        }
    }
}