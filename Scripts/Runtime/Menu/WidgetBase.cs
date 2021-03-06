﻿//vadym udod

using hootybird.Tools;
using hootybird.Tween;
using UnityEngine;
using UnityEngine.UI;

namespace hootybird.UI
{
    public abstract class WidgetBase : RoutinesBase
    {
        public bool initialized { get; private set; }
        public MenuScreen menuScreen { get; set; }

        public LayoutElement layoutElement { get; protected set; }
        public RectTransform rectTransform { get; protected set; }
        public CanvasGroup canvasGroup { get; protected set; }
        public ScaleTween scaleTween { get; protected set; }
        public PositionTween positionTween { get; protected set; }
        public AlphaTween alphaTween { get; protected set; }

        public bool visible => menuScreen.IsWidgetVisible(this);

        protected override void Awake()
        {
            Initialize();
        }

        public virtual void SetAlpha(float value)
        {
            if (canvasGroup)
                canvasGroup.alpha = value;
            else if (alphaTween)
                alphaTween.SetAlpha(value);
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
        }

        public virtual void MoveTo(Vector3 to, float time)
        {
            if (rectTransform)
                MoveTo(rectTransform.anchoredPosition, to, time);
            else
                MoveTo(transform.localPosition, to, time);
        }

        public virtual void MoveTo(Vector3 from, Vector3 to, float time)
        {
            if (positionTween)
            {
                positionTween.from = from;
                positionTween.to = to;
                positionTween.playbackTime = time;

                positionTween.PlayForward(true);
            }
        }

        public virtual void MoveRelative(Vector3 to, float time)
        {
            if (rectTransform)
                MoveTo(rectTransform.anchoredPosition, positionTween.to + to, time);
            else
                MoveTo(transform.localPosition, positionTween.to + to, time);
        }

        public virtual void ScaleTo(Vector3 to, float time) => ScaleTo(transform.localScale, to, time);

        public virtual void ScaleTo(Vector3 from, Vector3 to, float time)
        {
            if (scaleTween)
            {
                if (time == 0f)
                    scaleTween.SetScale(to);
                else
                {
                    scaleTween.from = from;
                    scaleTween.to = to;
                    scaleTween.playbackTime = time;

                    scaleTween.PlayForward(true);
                }
            }
        }

        public virtual void ResetAnchors()
        {
            rectTransform.anchorMin = rectTransform.anchorMax = Vector2.one * .5f;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public virtual void SetInteractable(bool state)
        {
            if (!canvasGroup) return;

            canvasGroup.interactable = state;
        }

        public virtual void BlockRaycast(bool state)
        {
            if (!canvasGroup) return;

            canvasGroup.blocksRaycasts = state;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        public virtual void _Update() { }

        public virtual void OnTap() { }

        public virtual void Initialize()
        {
            if (initialized) return;

            initialized = true;
            base.Awake();

            OnInitialized();
        }

        protected virtual void OnInitialized()
        {
            alphaTween = GetComponent<AlphaTween>();
            positionTween = GetComponent<PositionTween>();
            scaleTween = GetComponent<ScaleTween>();

            if (alphaTween) alphaTween.Initialize();
            if (positionTween) positionTween.Initialize();
            if (scaleTween) scaleTween.Initialize();

            rectTransform = GetComponent<RectTransform>();
            layoutElement = GetComponent<LayoutElement>();

            canvasGroup = GetComponent<CanvasGroup>();
            menuScreen = GetComponentInParent<MenuScreen>();
        }
    }
}
