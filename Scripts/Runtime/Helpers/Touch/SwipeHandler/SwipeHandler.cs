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
        public Action<SwipePoint, bool> onNewPointAdded;

        [SerializeField, Tooltip("Optional method to optimize swipe output data")]
        protected SwipeSolveMethod method = SwipeSolveMethod.BY_LAST_ANGLE;

        [SerializeField, Tooltip("New point is added only if pointer travelled X from previous point"), Range(.001f, .02f)]
        protected float pointDelta = .01f;

        [SerializeField]
        protected bool forceSwipeAtLength = false;

        [SerializeField, Tooltip("Force invoke swipe after given distance is travelled (0.05f-.5f)"), Range(.05f, .5f)]
        protected float swipeLength = .1f;

        [SerializeField]
        protected bool debugLastSwipe = false;

        private SwipePointsCollection collection = new SwipePointsCollection();
        private Swipe lastSwipe;
        private bool swipeEnded = false;
        private float _pointDelta;
        private float _swipeLength = -1f;

        protected void Awake()
        {
            Vector2 resolution = new Vector2(Screen.width, Screen.height);
            _pointDelta = resolution.magnitude * pointDelta;

            if (forceSwipeAtLength) _swipeLength = resolution.magnitude * swipeLength;
        }

        protected void OnDrawGizmos()
        {
            if (!debugLastSwipe) return;

            Color prev = Gizmos.color;

            if (collection.points.Count > 0)
            {
                Gizmos.color = Color.yellow;
                for (int pointIndex = 1; pointIndex < collection.points.Count; pointIndex++)
                    Gizmos.DrawLine(collection.points[pointIndex - 1].position, collection.points[pointIndex].position);
            }

            if (lastSwipe != null && lastSwipe.points.Count > 1)
            {
                Gizmos.color = Color.green;
                for (int pointIndex = 1; pointIndex < lastSwipe.points.Count; pointIndex++)
                    Gizmos.DrawLine(lastSwipe.points[pointIndex - 1].position, lastSwipe.points[pointIndex].position);
            }

            Gizmos.color = prev;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (eventData.pointerId > 0) return;

            collection.Add(eventData.position, Time.time);
            onNewPointAdded?.Invoke(collection.points.Last(), true);
            swipeEnded = false;

            onPointerDown?.Invoke(eventData.position);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            if (eventData.pointerId > 0 || swipeEnded) return;

            bool swipeDistanceTravelled = false;
            if (collection.points.Count > 0)
            {
                if ((eventData.position - collection.points.Last().position).magnitude >= _pointDelta)
                {
                    collection.Add(eventData.position, Time.time);

                    if (forceSwipeAtLength && SwipePointsCollection.GetPointsLength(collection.points) >= _swipeLength)
                        swipeDistanceTravelled = true;
                }

                onNewPointAdded?.Invoke(collection.points.Last(), false);
            }
            else
            {
                collection.Add(eventData.position, Time.time);

                onNewPointAdded?.Invoke(collection.points.Last(), true);
            }

            onPointerDrag?.Invoke(eventData.position);

            if (swipeDistanceTravelled) ForceSwipe();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            onPointerUp?.Invoke(eventData.position);

            if (eventData.pointerId > 0 || swipeEnded) return;

            InvokeFromCurrentPoints();
            swipeEnded = true;
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
            lastSwipe = Solve(method, collection.points);
            onSwipe?.Invoke(lastSwipe);

            if (clear) collection.Clear();
        }

        #region Static members

        public static Swipe Solve(SwipeSolveMethod method, List<SwipePoint> points)
        {
            List<SwipePoint> _points = new List<SwipePoint>();

            if (points.Count > 2)
            {
                float angle = 0f;
                int pointIndex;

                switch (method)
                {
                    case SwipeSolveMethod.BY_LAST_ANGLE:
                        pointIndex = points.Count - 1;

                        do
                        {
                            _points.Add(points[pointIndex]);
                            pointIndex--;
                        }
                        while (pointIndex >= 0 && (angle + points[pointIndex].angleDelta) < MAX_ANGLE_DEVIATION);

                        _points.Reverse();

                        break;

                    case SwipeSolveMethod.BY_LENGTH:
                        List<SwipePoint> currentGroup;

                        for (pointIndex = 0; pointIndex < points.Count; pointIndex++)
                        {
                            angle = 0f;
                            currentGroup = new List<SwipePoint>();

                            for (int groupIndex = pointIndex; groupIndex < points.Count; groupIndex++)
                            {
                                if (groupIndex > pointIndex) angle += points[groupIndex].angleDelta;

                                currentGroup.Add(points[groupIndex]);

                                if (Mathf.Abs(angle) > MAX_ANGLE_DEVIATION || groupIndex == points.Count - 1)
                                {
                                    if (SwipePointsCollection.GetPointsLength(currentGroup) >
                                        SwipePointsCollection.GetPointsLength(_points))
                                    {
                                        _points = new List<SwipePoint>(currentGroup);

                                        if (SwipePointsCollection.GetPointsLength(_points)
                                            >= (SwipePointsCollection.GetPointsLength(points) * .5f))
                                            return new Swipe(_points);
                                        else
                                            break;
                                    }
                                    else
                                        break;
                                }
                            }
                        }

                        break;

                    case SwipeSolveMethod.BY_LAST_ANGLE_NOT_CUMULATIVE:
                        int _index = points.Count - 1;

                        do
                        {
                            _points.Add(points[_index]);
                            _index--;
                        }
                        while (_index >= 0 && points[_index].angleDelta < MAX_ANGLE_DEVIATION);

                        _points.Reverse();

                        break;
                }
            }
            else
                _points = points;

            return new Swipe(_points);
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

        public void Add(Vector2 point, float time = 0f)
        {
            SwipePoint newPoint = new SwipePoint(point, time);
            SwipePoint prev = null;

            if (points.Count > 0) prev = points.Last();

            points.Add(newPoint);

            if (points.Count > 1)
            {
                prev.angle = Vector2.right.AngleTo((points[points.Count - 1].position - points[points.Count - 2].position));
                prev.timeDelta = time - prev.time;

                if (points.Count > 2) prev.angleDelta = prev.angle - points[points.Count - 3].angle;
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

            return length;
        }

        public static float GetSwipeTime(List<SwipePoint> points) => points.Sum(_point => _point.timeDelta);
    }

    public class SwipePoint
    {
        public Vector2 position;
        public float angle = 0f;
        public float angleDelta = 0f;
        public float time = 0f;
        public float timeDelta = 0f;

        public SwipePoint(Vector2 position, float timeAdded)
        {
            this.position = position;
            time = timeAdded;
        }
    }

    public enum SwipeSolveMethod
    {
        NONE,
        BY_LAST_ANGLE,
        BY_LENGTH,
        BY_LAST_ANGLE_NOT_CUMULATIVE,
    }
}