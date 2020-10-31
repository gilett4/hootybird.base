//@vadym udod

using hootybird.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace hootybird.UI.Helpers
{
    public class SwipeHandler : EventTrigger
    {
        internal const float MAX_ANGLE_DEVIATION = 30f;

        public Action<Swipe> onSwipe;
        public Action<Vector2> onPointerDown;
        public Action<Vector2> onPointerUp;
        public Action<Vector2> onPointerDrag;

        [SerializeField]
        protected SwipeSolveMethod method = SwipeSolveMethod.BY_LAST_ANGLE;
        [SerializeField]
        protected bool normalizeSwipePoints = true;

        private SwipePointsCollection collection = new SwipePointsCollection();
        private bool swipeEnded = false;

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            if (eventData.pointerId > 0) return;

            collection.Add(eventData.position, Time.time);

            onPointerDrag?.Invoke(eventData.position);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (eventData.pointerId > 0) return;

            collection.Add(eventData.position, Time.time);
            swipeEnded = false;

            onPointerDown?.Invoke(eventData.position);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            onPointerUp?.Invoke(eventData.position);

            if (eventData.pointerId > 0 || swipeEnded) return;

            InvokeFromCurrentPoints();
        }

        /// <summary>
        /// Get swipe from current set of swipe points.
        /// </summary>
        /// <returns></returns>
        public void ForceSwipe(bool endSwipe = true)
        {
            if (endSwipe) swipeEnded = true;

            InvokeFromCurrentPoints(true);
        }

        private void InvokeFromCurrentPoints(bool clear = true)
        {
            if (normalizeSwipePoints) collection.Scale(1f / Screen.width, 1f / Screen.height);

            onSwipe?.Invoke(Solve(method, collection.points));

            if (clear) collection.Clear();
        }

        #region Static members

        public static Swipe Solve(SwipeSolveMethod method, List<SwipePoint> points)
        {
            switch (method)
            {
                case SwipeSolveMethod.BY_LAST_ANGLE:
                    List<SwipePoint> newPoints = new List<SwipePoint>();
                    if (points.Count > 2)
                    {
                        float angleAcc = 0f;
                        int pointIndex = points.Count - 1;

                        do
                        {
                            newPoints.Add(points[pointIndex]);
                            pointIndex--;
                        }
                        while ((angleAcc + points[pointIndex].angleDelta) < MAX_ANGLE_DEVIATION && pointIndex >= 0);

                        newPoints.Reverse();
                    }
                    else
                        newPoints = points;

                    return new Swipe(newPoints);

                default:
                    return new Swipe(points);
            };
        }

        #endregion
    }

    public class Swipe : SwipePointsCollection
    {
        public bool IsHorizontal
        {
            get
            {
                float _angle = Angle;

                return (_angle > 315f || (_angle >= 0f && _angle < 45f)) || (_angle > 135f && _angle < 225f);
            }
        }

        public bool IsVertical => !IsHorizontal;

        public float Angle => points.Count > 1 ?
                        Vector2.right.AngleTo(points[points.Count - 1].position - points[points.Count - 2].position) : 0f;

        public float Length => GetPointsLength(points);

        public float TotalTime => GetSwipeTime(points);

        public Swipe() { }

        public Swipe(List<SwipePoint> points) : base(points) { }
    }

    public class SwipePointsCollection
    {
        public List<SwipePoint> points;

        public SwipePoint this[int index]
        {
            get => points[index];
        }

        public SwipePointsCollection()
        {
            points = new List<SwipePoint>();
        }

        public SwipePointsCollection(List<SwipePoint> points)
        {
            this.points = points;
        }

        public void Add(Vector2 point, float time = -1f)
        {
            SwipePoint swipePoint = new SwipePoint(point, time);
            points.Add(swipePoint);

            if (points.Count > 1)
            {
                swipePoint.angle =
                    Vector2.right.AngleTo((points[points.Count - 1].position - points[points.Count - 2].position));
                swipePoint.angleDelta = swipePoint.angle - points[points.Count - 2].angle;
                swipePoint.timeAdded = time;
                swipePoint.timeDelta = time - points[points.Count - 2].timeAdded;
            }
        }

        public void Clear()
        {
            points = new List<SwipePoint>();
        }

        public void Scale(Vector2 value) => points.ForEach(_point => _point.position.Scale(value));

        public void Scale(float x, float y) => Scale(new Vector2(x, y));

        public static float GetPointsLength(List<SwipePoint> points)
        {
            float length = 0f;

            if (points.Count > 1)
                for (int index = 1; index < points.Count; index++)
                    length += Vector2.Distance(points[index - 1].position, points[index].position);
            else if (points.Count == 1)
                length = points[0].position.magnitude;

            return length;
        }

        public static float GetSwipeTime(List<SwipePoint> points) => points.Sum(_point => _point.timeDelta);
    }

    public class SwipePoint
    {
        public Vector2 position;
        public float angle = 0f;
        public float angleDelta = 0f;
        public float timeAdded = 0f;
        public float timeDelta = 0f;

        public SwipePoint(Vector2 position, float timeAdded)
        {
            this.position = position;
            this.timeAdded = timeAdded;
        }
    }

    public enum SwipeSolveMethod
    {
        NONE,
        BY_LAST_ANGLE,
    }
}