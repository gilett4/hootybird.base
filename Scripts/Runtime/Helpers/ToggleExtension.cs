//@vadym udod

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace hootybird.Helpers
{
    public class ToggleExtension : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onToggleOn = default;
        [SerializeField]
        private UnityEvent onToggleOff = default;

        private Toggle toggle;

        protected void Awake()
        {
            toggle = GetComponent<Toggle>();

            toggle.onValueChanged.AddListener(OnToggle);
        }

        private void OnToggle(bool value)
        {
            if (value)
            {
                onToggleOn?.Invoke();
            }
            else
            {
                onToggleOff?.Invoke();
            }
        }
    }
}