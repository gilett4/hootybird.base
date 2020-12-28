//@vadym udod

using System.Collections.Generic;
using UnityEngine;

namespace hootybird.Tween
{
    public class AlphaTween : TweenBase
    {
        public Transform[] customObjects;
        public bool propagate = false;
        public float from = 0f;
        public float to = 1f;

        public List<Graphics> alphaGroup { get; private set; }

        /// <summary>
        /// Actual alpha value
        /// </summary>
        public float _value { get; private set; }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            float currentValue;
            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        currentValue = Mathf.Lerp(from, to, curve.Evaluate(value));
                    else
                    {
                        currentValue = Mathf.Lerp(from, to, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;

                default:
                    if (value < 1f)
                        currentValue = Mathf.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        currentValue = Mathf.Lerp(to, from, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;
            }

            SetAlpha(currentValue);
        }

        public void SetAlpha(float value)
        {
            _value = value;
            foreach (Graphics group in alphaGroup) group.alpha = value;
        }

        public override void OnReset()
        {
            SetAlpha(from);
        }

        public override void OnInitialized()
        {
            DoParse(customObjects, propagate);

            if (alphaGroup.Count > 0) _value = alphaGroup[0].alpha;
        }
    }
}