//@vadym udod

using UnityEngine;
using UnityEngine.UI;

namespace hootybird.Tween
{
    [RequireComponent(typeof(Image))]
    public class FillTween : TweenBase
    {
        public float from = 0f;
        public float to = 1f;

        private Image image;

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

            SetFillValue(currentValue);
        }

        public void SetFillValue(float value)
        {
            image.fillAmount = _value = value;
        }

        public override void OnReset()
        {
            SetFillValue(from);
        }

        public override void OnInitialized()
        {
            image = GetComponent<Image>();

            if (image) _value = image.fillAmount;
        }
    }
}
