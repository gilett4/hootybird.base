//@vadym udod

using UnityEngine;

namespace hootybird.Tween
{
    public class BezierPositionTween : TweenBase
    {
        public Vector2 from;
        public Vector2 to;
        public Vector2 control;

        private RectTransform rectTransform;

        public Vector3 _value { get; private set; }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            Vector3 currentPosition;
            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        currentPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(value));
                    else
                    {
                        currentPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;
                default:
                    if (value < 1f)
                        currentPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(1f - value));
                    else
                    {
                        currentPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;
            }

            SetPosition(currentPosition);
        }

        public void SetPosition(Vector3 position)
        {
            _value = position;

            if (rectTransform)
                rectTransform.anchoredPosition = position;
            else
                transform.localPosition = position;
        }

        public static Vector2 GetBezierPoint(Vector2 start, Vector2 control, Vector2 end, float time)
        {
            Vector2 t1 = Vector2.Lerp(start, control, time);
            Vector2 t2 = Vector2.Lerp(control, end, time);

            return Vector2.Lerp(t1, t2, time);
        }

        public override void OnReset()
        {
            SetPosition(to);
        }

        public override void OnInitialized()
        {
            rectTransform = GetComponent<RectTransform>();

            if (rectTransform) 
                _value = rectTransform.anchoredPosition;
            else 
                _value = transform.localPosition;
        }
    }
}