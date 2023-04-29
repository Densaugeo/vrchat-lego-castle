/*
 * Ran using `D:\unity\editor\Unity.exe -projectPath
 * D:\unity-projects\vrchat-lego-castle -executeMethod PrefabColliders.Generate`
 * Docs recommend adding -batchmode and -quit options for full auto
 * 
 * To get Intellisense working:
 * - Installed C# extension
 * - Installed .NET 7.0 SDK (current latest)
 * - Installed .NET 4.7.1 developer pack
 * - Set Unity's script editor to VSCode in Edit > Preferences > External Tools
 * - VSCode must be opened by selecting a script in Unity and clicking "open" in
 *   the inspector
 * - Assembly-CSharp.csproj, Assembly-CSharp-Editor.csproj, and
 *   .vscode/settings.json must be presernt. If they are missing, they can be
 *   created in Unity > Edit > Preferences > Regenerate project files (I left
 *   all of the associated checkboxes empty)
 */

using UnityEngine;
using UnityEditor;

public class PrefabColliders : MonoBehaviour {
    static void Generate() {
        foreach(var file in new System.IO.DirectoryInfo("Assets/blender").
            GetFiles("*.fbx")) {
            string prefab_path = "Assets/blender/" +
                System.IO.Path.GetFileNameWithoutExtension(file.Name) +
                ".prefab";
            bool success;
            
            System.IO.File.Delete(prefab_path);
            System.IO.File.Delete(prefab_path + ".meta");
            
            var fbx = (GameObject) AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/blender/" + file.Name);
            var instance = (GameObject) PrefabUtility.InstantiatePrefab(fbx);
            PrefabUtility.SaveAsPrefabAsset(instance, prefab_path, out success);
            // Regular destroy can't be called in edit mode
            GameObject.DestroyImmediate(instance);
            System.Diagnostics.Trace.Assert(success);
            
            var prefab = PrefabUtility.LoadPrefabContents(prefab_path);
            for(int i = 0; i < prefab.transform.childCount; ++i) {
                var child = prefab.transform.GetChild(i).gameObject;
                
                if(child.name.StartsWith("collider-")) {
                    child.GetComponent<MeshRenderer>().enabled = false;
                    child.AddComponent<MeshCollider>();
                }
            }
            PrefabUtility.SaveAsPrefabAsset(prefab, prefab_path, out success);
            PrefabUtility.UnloadPrefabContents(prefab);
            System.Diagnostics.Trace.Assert(success);
        }
        
        AssetDatabase.Refresh();
    }
}
