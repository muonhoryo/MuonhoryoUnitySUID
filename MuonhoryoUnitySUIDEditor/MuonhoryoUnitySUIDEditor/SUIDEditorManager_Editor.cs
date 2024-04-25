
using System.Text;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace MuonhoryoLibrary.Unity.SUID.Editor
{
    [CustomEditor(typeof(SUIDEditorManager))]
    internal sealed class SUIDEditorManager_Editor:UnityEditor.Editor
    {

        private SUIDEditorManager ParsedOwner;

        private static string SUIDObjectsInfo = "";

        public override VisualElement CreateInspectorGUI()
        {
            ParsedOwner = target as SUIDEditorManager;
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
            if (!ParsedOwner.TryGetComponent(out SUIDManager obj))
            {
                Debug.LogError("Missing SUIDManager.");
                DestroyImmediate(ParsedOwner);
                return;
            }

            base.OnInspectorGUI();
            EditorGUILayout.TextField("SUID count ",
                 ParsedOwner.InSceneObjectsCount_.ToString());
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
