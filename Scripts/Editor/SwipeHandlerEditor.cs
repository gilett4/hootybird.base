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
        private SerializedProperty normalizeProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            methodProperty = serializedObject.FindProperty("method");
            normalizeProperty = serializedObject.FindProperty("normalizeSwipePoints");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(methodProperty);
            EditorGUILayout.PropertyField(normalizeProperty);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
#endif
