﻿//@vadym udod

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace hootybird.UI.Helpers.Touch
{
    public class TouchZone : EventTrigger
    {
        public UnityEventVector2 onPointerDown;
        public UnityEventVector2 onPointerMove;
        public UnityEventVector2 onPointerUp;

        public bool dragPositionRelative = false;
        public bool scalePosition = false;

        private Vector2 originPosition;
        private Canvas canvas;

        public void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (scalePosition)
                originPosition = new Vector2(
                    eventData.position.x / canvas.transform.lossyScale.x,
                    eventData.position.y / canvas.transform.lossyScale.y);
            else
                originPosition = new Vector2(eventData.position.x, eventData.position.y);

            onPointerDown.Invoke(originPosition);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            Vector2 _value = Vector2.zero;
            if (dragPositionRelative)
            {
                if (scalePosition)
                    _value = new Vector2(
                        eventData.position.x / canvas.transform.lossyScale.x,
                        eventData.position.y / canvas.transform.lossyScale.y) - originPosition;
                else
                    _value = new Vector2(eventData.position.x, eventData.position.y) - originPosition;
            }
            else
            {
                if (scalePosition)
                    _value = new Vector2(
                        eventData.position.x / canvas.transform.lossyScale.x,
                        eventData.position.y / canvas.transform.lossyScale.y);
                else
                    _value = new Vector2(eventData.position.x, eventData.position.y);
            }

            onPointerMove.Invoke(_value);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (scalePosition)
                onPointerUp.Invoke(new Vector2(
                    eventData.position.x / canvas.transform.lossyScale.x,
                    eventData.position.y / canvas.transform.lossyScale.y));
            else
                onPointerUp.Invoke(new Vector2(eventData.position.x, eventData.position.y));
        }
    }

    [System.Serializable]
    public class UnityEventVector2 : UnityEvent<Vector2> { }
}