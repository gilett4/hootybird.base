﻿//@vadym udod

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace hootybird.Helpers
{
    [ExecuteInEditMode]
    public class OnRatio : MonoBehaviour
    {
        public bool continious = false;

        public DeviceOrientation orientation = DeviceOrientation.Portrait;
        public UnityEvent onIphone;
        public UnityEvent onIphoneX;
        public UnityEvent onIpad;

        private DisplayRatioOption ratio = DisplayRatioOption.NONE;
        private DisplayRatioOption previousRatio = DisplayRatioOption.NONE;

        private float previousAspect;

        protected void Start()
        {
            if (!Application.isPlaying) return;

            CheckOrientation();
        }

        protected void Update()
        {
            if (continious || Application.isEditor) CheckOrientation();
        }

        public void SetHeight(float value)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (rectTransform)
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, value);
        }

        public void SetWidth(float value)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (rectTransform)
                rectTransform.sizeDelta = new Vector2(value, rectTransform.sizeDelta.y);
        }

        public void SetPaddingLeft(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
                layoutGroup.padding = new RectOffset(value, layoutGroup.padding.right, layoutGroup.padding.top, layoutGroup.padding.bottom);
        }

        public void SetPaddingRight(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
                layoutGroup.padding = new RectOffset(layoutGroup.padding.left, value, layoutGroup.padding.top, layoutGroup.padding.bottom);
        }

        public void SetPaddingTop(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
                layoutGroup.padding = new RectOffset(layoutGroup.padding.left, layoutGroup.padding.right, value, layoutGroup.padding.bottom);
        }

        public void SetPaddingBottom(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
                layoutGroup.padding = new RectOffset(layoutGroup.padding.left, layoutGroup.padding.right, layoutGroup.padding.top, value);
        }

        public void CheckOrientation()
        {
            //ratio check
            switch (orientation)
            {
                case DeviceOrientation.LandscapeLeft:
                case DeviceOrientation.LandscapeRight:
                    if (Camera.main.aspect > 1.85f)
                    {
                        ratio = DisplayRatioOption.IPHONEX;
                        UpdatePrevious();
                    }
                    else if (Camera.main.aspect > 1.5f)
                    {
                        ratio = DisplayRatioOption.IPHONE;
                        UpdatePrevious();
                    }
                    else if (Camera.main.aspect > 1f)
                    {
                        ratio = DisplayRatioOption.IPAD;
                        UpdatePrevious();
                    }

                    break;

                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    if (Camera.main.aspect <= 1f)
                    {
                        if (Camera.main.aspect >= .7f)
                        {
                            ratio = DisplayRatioOption.IPAD;
                            UpdatePrevious();
                        }
                        else if (Camera.main.aspect >= .5f)
                        {
                            ratio = DisplayRatioOption.IPHONE;
                            UpdatePrevious();
                        }
                        else
                        {
                            ratio = DisplayRatioOption.IPHONEX;
                            UpdatePrevious();
                        }
                    }

                    break;
            }

            previousAspect = Camera.main.aspect;
        }

        private void OnRatioChanged(DisplayRatioOption option)
        {
            if (onIphone == null || onIphoneX == null || onIpad == null) return;

            switch (option)
            {
                case DisplayRatioOption.IPHONE:
                    onIphone.Invoke();
                    break;

                case DisplayRatioOption.IPHONEX:
                    onIphoneX.Invoke();
                    break;

                case DisplayRatioOption.IPAD:
                    onIpad.Invoke();
                    break;
            }
        }

        private void UpdatePrevious()
        {
            if (Application.isEditor)
            {
                if (previousAspect != Camera.main.aspect)
                {
                    OnRatioChanged(ratio);
                    previousRatio = ratio;
                }
            }
            else if (ratio != previousRatio)
            {
                OnRatioChanged(ratio);
                previousRatio = ratio;
            }
        }

        public enum DisplayRatioOption
        {
            NONE,

            IPHONE,
            IPHONEX,
            IPAD,
        }
    }
}