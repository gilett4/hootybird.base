//@vady udod

using ByteSheep.Events;
using hootybird.audio;
using hootybird.Serialized;
using hootybird.Tween;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NaughtyAttributes;

namespace hootybird.UI.Helpers
{
    public class ButtonExtended : Button
    {
        public static Material GREYSCALE_MATERIAL;

        public AdvancedEvent events;
        public AdvancedEvent onState;
        public AdvancedEvent offState;
        public string sfxOnClick = "button_click_1";
        public bool changeMaterialOnState = true;
        public bool scaleOnClick = true;
        [ShowIf("changeMaterialOnState")]
        public List<ButtonGraphics> buttonGraphics;

        public Dictionary<string, TMP_Text> labelsFastAccess { get; private set; }
        public Dictionary<string, Badge> badgesFastAccess { get; private set; }

        public ScaleTween scaleTween { get; set; }
        public AlphaTween alphaTween { get; set; }
        public List<MaskableGraphic> maskableGraphics { get; private set; }

        private bool initialized = false;

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;

            Initialize();
        }

        protected override void Start()
        {
            base.Start();

            if (!Application.isPlaying) return;

            if (interactable) onState.Invoke();
        }

        public TMP_Text GetLabel(string _name = null)
        {
            Initialize();

            if (string.IsNullOrEmpty(_name))
                return labelsFastAccess.Values.First();
            else if (labelsFastAccess.ContainsKey(_name)) return labelsFastAccess[_name];

            return null;
        }

        public void SetLabel(string value, string labelName = null) => GetLabel(labelName)?.SetText(value);

        public Badge GetBadge(string _name = null)
        {
            Initialize();

            if (string.IsNullOrEmpty(_name))
                return badgesFastAccess.Values.First();
            if (badgesFastAccess.ContainsKey(_name)) return badgesFastAccess[_name];

            return null;
        }

        public void SetActive(bool state) => gameObject.SetActive(state);

        public void SetMaterial(Material material)
        {
            foreach (MaskableGraphic maskable in maskableGraphics)
                maskable.material = material;
        }

        public void SetState(bool state)
        {
            interactable = state;

            if (changeMaterialOnState) SetMaterial(state ? null : GREYSCALE_MATERIAL);

            if (state) onState.Invoke();
            else offState.Invoke();
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

        public void Parse(Transform obj, bool propagate)
        {
            MaskableGraphic maskable = obj.GetComponent<MaskableGraphic>();

            if (!maskable) return;

            maskableGraphics.Add(maskable);

            if (propagate)
                for (int i = 0; i < obj.childCount; i++)
                    Parse(obj.GetChild(i), propagate);
        }

        public virtual void Hide(float time)
        {
            if (time == 0f)
                alphaTween.SetAlpha(0f);
            else
            {
                alphaTween.playbackTime = time;
                alphaTween.PlayBackward(true);
            }
        }

        public virtual void Show(float time)
        {
            if (time == 0f)
                alphaTween.SetAlpha(1f);
            else
            {
                alphaTween.playbackTime = time;
                alphaTween.PlayForward(true);
            }
        }

        public virtual void ScaleTo(Vector3 to, float time) => ScaleTo(transform.localScale, to, time);

        public virtual void ScaleTo(Vector3 from, Vector3 to, float time)
        {
            scaleTween.from = from;
            scaleTween.to = to;
            scaleTween.playbackTime = time;

            scaleTween.PlayForward(true);
        }

        private void OnClick()
        {
            events.Invoke();

            AudioHolder.Instance.PlaySelfSfxOneShotTracked(sfxOnClick);

            if (scaleOnClick && scaleTween) scaleTween.PlayForward(true);
        }

        private void Initialize()
        {
            if (!Application.isPlaying || initialized)
                return;

            initialized = true;

            //add our events to onClick events
            onClick.AddListener(OnClick);

            //get labels/badges
            labelsFastAccess = new Dictionary<string, TMP_Text>();
            foreach (TMP_Text label in GetComponentsInChildren<TMP_Text>())
                if (!labelsFastAccess.ContainsKey(label.name))
                    labelsFastAccess.Add(label.name, label);

            badgesFastAccess = new Dictionary<string, Badge>();
            foreach (Badge badge in GetComponentsInChildren<Badge>())
                if (!badgesFastAccess.ContainsKey(badge.name))
                    badgesFastAccess.Add(badge.name, badge);

            //get tweens
            scaleTween = GetComponent<ScaleTween>();
            alphaTween = GetComponent<AlphaTween>();

            DoParse();

            if (!GREYSCALE_MATERIAL) GREYSCALE_MATERIAL = new Material(Shader.Find("Custom/GreyscaleUI"));
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
}