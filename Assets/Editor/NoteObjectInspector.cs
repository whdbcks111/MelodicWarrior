using _02.Scripts.Level.Note;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(NoteObject))]
    public class NoteObjectInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var noteObject = (NoteObject)target;
            
            EditorGUILayout.LabelField("Appear Beat", $"{noteObject.note?.appearBeat ?? 0f}");
        }
    }
}