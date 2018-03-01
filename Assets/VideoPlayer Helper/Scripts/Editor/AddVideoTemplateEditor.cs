using UnityEditor;
using UnityEngine;

namespace Unity.VideoHelper.Editor
{
    public class AddVideoTemplateEditor : EditorWindow
    {
        private const string TemplatePath = "Assets/VideoPlayer Helper/Prefabs/Template.prefab";
        private const string TemplateName = "Video Player with Controls";

        [MenuItem("GameObject/Video/Video Player with Controls", priority = 10)]
        public static void Create()
        {
            var parent = Selection.activeGameObject == null ? null : Selection.activeGameObject.transform;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TemplatePath);
            if (prefab != null)
            {
                var instance = Instantiate(prefab, parent);
                instance.name = TemplateName;
            }
        }
    }
}
