//@vadym udod

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

namespace hootybird.UI.Helpers
{
    [CustomEditor(typeof(SliderExtended), true)]
    public class SliderExtendedEditor : SliderEditor
    {
        private SerializedProperty onValueProperty;
        private SerializedProperty formatProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            onValueProperty = serializedObject.FindProperty("onValueChangeString");
            formatProperty = serializedObject.FindProperty("format");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(onValueProperty);
            EditorGUILayout.PropertyField(formatProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif