//@vadym udod

using UnityEditor;
using UnityEditor.UI;

namespace hootybird.UI.Helpers
{
    [CustomEditor(typeof(ButtonExtended), true)]
    public class ButtonExtendedEditor : ButtonEditor
    {
        private SerializedProperty eventProperty;
        private SerializedProperty sfxProperty;
        private SerializedProperty buttonGraphicsProperty;
        private SerializedProperty changeMaterialOnStateProperty;
        private SerializedProperty scaleOnClickProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            eventProperty = serializedObject.FindProperty("events");
            sfxProperty = serializedObject.FindProperty("playOnClick");
            buttonGraphicsProperty = serializedObject.FindProperty("buttonGraphics");
            changeMaterialOnStateProperty = serializedObject.FindProperty("changeMaterialOnState");
            scaleOnClickProperty = serializedObject.FindProperty("scaleOnClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(eventProperty);
            EditorGUILayout.PropertyField(sfxProperty);
            EditorGUILayout.PropertyField(changeMaterialOnStateProperty);
            EditorGUILayout.PropertyField(scaleOnClickProperty);
            EditorGUILayout.PropertyField(buttonGraphicsProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
