//@vadynm udod

using hootybird.Tween;
using hootybird.UI.Helpers;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace hootybird.Helpers
{
    [RequireComponent(typeof(Button))]
    public class ButtonExtension : MonoBehaviour
    {
        public static Material GREYSCALE_MATERIAL;

        [SerializeField]
        internal string sfxOnClick = "button_click_1";
        [SerializeField]
        internal bool changeMaterialOnState = true;
        [SerializeField, ShowIf("changeMaterialOnState"), ReorderableList]
        internal List<ButtonGraphics> buttonGraphics = default;

        private Dictionary<string, TMP_Text> labels;
        private Dictionary<string, Badge> badges;
        private List<MaskableGraphic> maskableGraphics;
        private ScaleTween scaleTween;
        private AlphaTween alphaTween;
        private CanvasGroup canvasGroup;

        private bool initialized = false;

        public Button Button { get; private set; }

        protected void Awake()
        {
            Initialize();
        }

        public virtual void Scale(Vector3 to, float time) => Scale(transform.localScale, to, time);

        public virtual void Scale(Vector3 from, Vector3 to, float time)
        {
            if (!scaleTween)
            {
                scaleTween.from = from;
                scaleTween.to = to;
                scaleTween.playbackTime = time;

                scaleTween.PlayForward(true);
            }
            else
                transform.localScale = to;
        }

        public virtual void Hide(float time)
        {
            if (alphaTween)
            {
                if (time == 0f)
                    alphaTween.SetAlpha(0f);
                else
                {
                    alphaTween.playbackTime = time;
                    alphaTween.PlayBackward(true);
                }
            }
            else if (canvasGroup)
                canvasGroup.alpha = 0f;

        }

        public virtual void Show(float time)
        {
            if (alphaTween)
            {
                if (time == 0f)
                    alphaTween.SetAlpha(1f);
                else
                {
                    alphaTween.playbackTime = time;
                    alphaTween.PlayForward(true);
                }
            }
            else if (canvasGroup)
                canvasGroup.alpha = 1f;
        }

        public void DoParse()
        {
            if (buttonGraphics == null) return;

            maskableGraphics = new List<MaskableGraphic>();

            if (buttonGraphics.Count != 0)
                foreach (ButtonGraphics obj in buttonGraphics)
                    Parse(obj.target.transform, obj.propagate);
            else
                Parse(transform, true);
        }
        public TMP_Text GetLabel(string _name = null)
        {
            Initialize();

            if (string.IsNullOrEmpty(_name))
                return labels.Values.First();
            else if (labels.ContainsKey(_name))
                return labels[_name];

            return null;
        }

        public void SetLabel(string value, string _name = null) => 
            GetLabel(_name)?.SetText(value);

        public Badge GetBadge(string _name = null)
        {
            Initialize();

            if (string.IsNullOrEmpty(_name))
                return badges.Values.First();
            if (badges.ContainsKey(_name)) 
                return badges[_name];

            return null;
        }

        public void SetBadge(string value, string _name = null) =>
            GetBadge(_name)?.SetValue(value);

        /// <summary>
        /// Enable/Disable GO
        /// </summary>
        /// <param name="state"></param>
        public void SetActive(bool state) => gameObject.SetActive(state);

        public void SetMaterial(Material material)
        {
            foreach (MaskableGraphic maskable in maskableGraphics)
                maskable.material = material;
        }

        public void SetState(bool state)
        {
            if (Button) Button.interactable = state;

            if (changeMaterialOnState) SetMaterial(state ? null : GREYSCALE_MATERIAL);
        }

        private void Parse(Transform obj, bool propagate)
        {
            MaskableGraphic maskable = obj.GetComponent<MaskableGraphic>();

            if (!maskable) return;

            maskableGraphics.Add(maskable);

            if (propagate)
                for (int i = 0; i < obj.childCount; i++)
                    Parse(obj.GetChild(i), propagate);
        }

        private void Initialize()
        {
            if (initialized) return;

            Button = GetComponent<Button>();

            alphaTween = GetComponent<AlphaTween>();
            scaleTween = GetComponent<ScaleTween>();
            canvasGroup = GetComponent<CanvasGroup>();

            labels = new Dictionary<string, TMP_Text>();
            foreach (TMP_Text label in GetComponentsInChildren<TMP_Text>())
                if (!labels.ContainsKey(label.name))
                    labels.Add(label.name, label);

            badges = new Dictionary<string, Badge>();
            foreach (Badge badge in GetComponentsInChildren<Badge>())
                if (!badges.ContainsKey(badge.name))
                    badges.Add(badge.name, badge);

            DoParse();

            initialized = true;
        }
    }

    [Serializable]
    public class ButtonGraphics
    {
        [HideInInspector]
        public string _name;

        [ShowIf("Check")]
        public GameObject target;
        public bool propagate;

        private bool Check()
        {
            if (!target) return true;

            _name = target.name;

            return true;
        }
    }
}
