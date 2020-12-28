//@vadym udod

using UnityEngine;

namespace hootybird.Tween
{
    public class SquishStretchTween : TweenBase
    {
        public float xAxisPower = -1f;
        public float yAxisPower = 1f;

        public Vector3 _value { get; private set; }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            Vector3 currentValue;
            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        currentValue = Vector3.one +
                            new Vector3(curve.Evaluate(value) * xAxisPower, curve.Evaluate(value) * yAxisPower);
                    else
                    {
                        currentValue = Vector3.one + 
                            new Vector3(curve.Evaluate(1f) * xAxisPower, curve.Evaluate(1f) * yAxisPower);
                        isPlaying = false;
                    }
                    break;

                default:
                    if (value < 1f)
                        currentValue = Vector3.one +
                            new Vector3(
                                curve.Evaluate(1f - value) * xAxisPower,
                                curve.Evaluate(1f - value) * yAxisPower);
                    else
                    {
                        currentValue = Vector3.one + 
                            new Vector3(curve.Evaluate(0f) * xAxisPower, curve.Evaluate(0f) * yAxisPower);
                        isPlaying = false;
                    }
                    break;
            }

            SetValue(currentValue);
        }

        public void SetValue(Vector3 value)
        {
            transform.localScale = _value = value;
        }

        public override void OnReset()
        {
            SetValue(Vector3.one);
        }

        public override void OnInitialized() { }
    }
}
