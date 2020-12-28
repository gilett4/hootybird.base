//@vadym udod

using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace hootybird.UI.Helpers
{
    public class Badge : MonoBehaviour
    {
        [SerializeField]
        internal bool hideOnEmpty = true;
        [SerializeField]
        internal bool thisTarget = true;
        [SerializeField]
        internal string format = "{0}";
        
        [ShowIf("ShowCheck"), SerializeField]
        internal GameObject targetObject = default;
        [ShowIf("ShowCheck"), SerializeField]
        internal TextMeshProUGUI targetText = default;
        
        private bool initialized = false;

        protected void Awake()
        {
            Initialize();
        }

        public void SetValue(string value)
        {
            Initialize();
            
            if (!targetObject) return;

            if (hideOnEmpty)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Hide();

                    return;
                }

                if (int.TryParse(value, out int intValue) && intValue == 0)
                {
                    Hide();

                    return;
                }
            }

            if (!targetText) return;

            Show();

            targetText.text = string.Format(format, value);
        }

        public void SetValue(float value) => SetValue(value.ToString());

        public void SetValue(int value) => SetValue(value.ToString());

        public void Hide()
        {
            Initialize();

            if (!targetObject) return;

            targetObject.SetActive(false);
        }

        public void Show()
        {
            Initialize();

            if (!targetObject) return;

            targetObject.SetActive(true);
        }

        public void SetState(bool state)
        {
            if (state)
                Show();
            else
                Hide();
        }

        private void Initialize()
        {
            if (initialized) return;

            if (thisTarget) targetObject = gameObject;

            if (!targetText) targetText = targetObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        //editor
        private bool ShowCheck() => !thisTarget;
    }
}
