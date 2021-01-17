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

        public bool isOn
        {
            get
            {
                if (!CheckRequired())
                {
                    return false;
                }

                return toggle.isOn;
            }

            set
            {
                if (!CheckRequired())
                {
                    return;
                }

                toggle.isOn = value;
            }
        }

        protected void Awake()
        {
            toggle = GetComponent<Toggle>();

            toggle.onValueChanged.AddListener(OnToggle);
        }

        public void SetIsOnWithoutNotify(bool value)
        {
            if (!CheckRequired()) return;

            toggle.SetIsOnWithoutNotify(value);
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

        private bool CheckRequired()
        {
            if (!toggle)
            {
                Debug.Log($"{name} requires Toggle component");
            }

            return toggle;
        }
    }
}