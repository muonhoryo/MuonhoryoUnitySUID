

using System.Text;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace MuonhoryoLibrary.Unity.SUID.Editor
{
    [CustomEditor(typeof(SUIDObjectEditorInitializer))]
    internal sealed class SUIDObjectEditorInitializer_Editor:UnityEditor.Editor
    {

        private SUIDObjectEditorInitializer ParsedOwner;

        private static string SUIDObjectsInfo = "";

        public override VisualElement CreateInspectorGUI()
        {
            ParsedOwner = target as SUIDObjectEditorInitializer;
            return base.CreateInspectorGUI();
        }

        private string GetObjectsInfo()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i <= SUIDEditorManager.Instance_.InSceneObjectsCount_; i++)
            {
                sb.AppendLine($"{i}:  {SUIDEditorManager.Instance_.GetObjectByID(i).gameObject.name}");
            }
            return sb.ToString();
        }
        public override void OnInspectorGUI()
        {
            if (!ParsedOwner.TryGetComponent(out SUIDObject obj))
            {
                Debug.LogError("Missing SUIDObject.");
                DestroyImmediate(ParsedOwner);
                return;
            }

            base.OnInspectorGUI();
            EditorGUILayout.TextField("SUID",
                ParsedOwner.ID_ + "  /  " + SUIDEditorManager.Instance_.InSceneObjectsCount_);

            if (SUIDEditorManager.Instance_ == null)
                throw new System.NullReferenceException("Haven't SUIDEditorManager in scene.");

            if (SUIDEditorManager.Instance_.GetObjectByID(ParsedOwner.ID_) == null)
                SUIDEditorManager.Instance_.RunSUIDObjDelayedInitializations();
            SUIDObjectsInfo = GetObjectsInfo();
            EditorGUILayout.TextArea(SUIDObjectsInfo);
        }

        private void OnDestroy()
        {
            if (ParsedOwner != null && (ParsedOwner.gameObject.hideFlags & HideFlags.NotEditable) == 0) //Return editing lock
                ParsedOwner.SetHideFlags((ParsedOwner.gameObject.hideFlags | HideFlags.NotEditable));
        }
    }
}
