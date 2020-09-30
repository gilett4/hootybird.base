//@vadym udod

using hootybird.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace hootybird.UI
{
    public class SwipeHandlerAdvanced : MonoBehaviour
    {
        public Action<SwipeHandlerAdvanced> onSwipe;
        public float maxSwipeTime = .3f;

        public List<SwipeHandlerPoint> lastPoints { get; private set; } = new List<SwipeHandlerPoint>();
        public Vector2 lastPosition { get; private set; }
        public SwipeHandlerPoint lastPoint { get; private set; }
        public float lastSwipeTime { get; private set; }

        private bool touched = false;
        private bool useMouse = false;

        private float startTime;
        private float maxSwipeDistance = Screen.height * .4f;

        protected void Update()
        {
            if (!touched) return;

            if (Time.time - startTime >= maxSwipeTime)
            {
                useMouse = false;
                touched = false;
                return;
            }

            UpdateLastPosition();

            if (lastPosition == lastPoint.position) return;

            AddPoint(lastPosition);
        }

        public void OnPDown()
        {
            if (touched) return;

            touched = true;
            startTime = Time.time;

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) useMouse = true;
            UpdateLastPosition();

            lastPoints.Clear();
            AddPoint(lastPosition);
        }

        public void OnPUp()
        {
            if (Input.touchCount > 1 || !touched) return;

            useMouse = false;
            touched = false;
            lastSwipeTime = Time.time - startTime;

            onSwipe?.Invoke(this);
        }

        public float LastSwipeLength()
        {
            if (lastPoints.Count < 2) return 0f;

            float length = 0f;

            for (int pointIndex = 1; pointIndex < lastPoints.Count; pointIndex++)
                length += Vector2.Distance(lastPoints[pointIndex].position, lastPoints[pointIndex - 1].position);

            return length;
        }

        public float LastSwipeLengthRelative()
        {
            if (lastPoints.Count < 2) return 0f;

            return Mathf.Clamp01(Vector2.Distance(lastPoint.position, lastPoints[0].position) / maxSwipeDistance);
        }

        public float LastSwipeAngle()
        {
            float angle = 0f;

            if (lastPoints.Count == 1)
                return lastPoint.angle;
            else if (lastPoints.Count > 2)
                for (int pointIndex = 0; pointIndex < lastPoints.Count - 2; pointIndex++)
                    angle += lastPoints[pointIndex + 1].angle - lastPoints[pointIndex].angle;

            return angle;
        }

        public float LastSwipeDirection()
        {
            if (lastPoints.Count < 2) return 0f;
            else return Vector2.right.AngleTo(lastPoint.position - lastPoints[0].position);
        }

        protected void UpdateLastPosition()
        {
            if (useMouse)
                lastPosition = Input.mousePosition;
            else
                lastPosition = Input.GetTouch(0).position;
        }

        protected void AddPoint(Vector2 position)
        {
            lastPoint = new SwipeHandlerPoint() { position = position };

            if (lastPoints.Count > 0) lastPoints[lastPoints.Count - 1].SetAngleTo(lastPoint);

            lastPoints.Add(lastPoint);
        }

        public class SwipeHandlerPoint
        {
            public Vector2 position;
            public float angle;

            public void SetAngleTo(SwipeHandlerPoint point) => angle = Vector2.right.AngleTo(point.position - position);
        }
    }
}