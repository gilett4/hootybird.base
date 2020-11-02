//@vadym udod

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.EventSystems;

namespace hootybird.UI.Helpers
{
    [CustomEditor(typeof(SwipeHandler))]
    public class SwipeHandlerEditor : EventTriggerEditor
    {
        private SerializedProperty methodProperty;
        private SerializedProperty forceSwipeAtLengthProperty;
        private SerializedProperty swipeLengthProperty;
        private SerializedProperty debugLastSwipeProperty;
        private SerializedProperty pointDeltaProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            methodProperty = serializedObject.FindProperty("method");
            pointDeltaProperty = serializedObject.FindProperty("pointDelta");
            forceSwipeAtLengthProperty = serializedObject.FindProperty("forceSwipeAtLength");
            swipeLengthProperty = serializedObject.FindProperty("swipeLength");
            debugLastSwipeProperty = serializedObject.FindProperty("debugLastSwipe");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(methodProperty);
            EditorGUILayout.PropertyField(pointDeltaProperty);
            EditorGUILayout.PropertyField(forceSwipeAtLengthProperty);

            if (forceSwipeAtLengthProperty.boolValue) EditorGUILayout.PropertyField(swipeLengthProperty);

            EditorGUILayout.PropertyField(debugLastSwipeProperty);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
#endif
