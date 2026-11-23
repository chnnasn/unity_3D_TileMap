using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

#if (UNITY_EDITOR)

namespace MAST
{
    public static class LoadingHelper
    {
        const string imagePath = "/Images/";
        const string guiPath = "/Etc/GUISkin.guiskin";
        const string gridMaterialPath = "/Etc/Material_Grid.mat";
        const string paintAreaMaterialPath = "/Etc/Material_PaintArea.mat";
        const string eraserPrefabPath = "/Etc/Prefab_Eraser.prefab";
        const string thumbnailCameraPath = "/Etc/Prefab_Preview_Camera.prefab";

        // Folder name used when the tool is installed inside the Assets folder instead of as a package
        const string assetFolderName = "TileMap3D";

        // Folder name used by the legacy (pre-package) layout
        const string legacyFolderName = "MAST";

        // Cached result of the tool root lookup
        static string cachedToolRoot = null;

        public static string ConvertAbsolutePathToProjectPath(string path)
        {
            // If this absolute path is for this project, replace application data path with "Assets"
            if (path.StartsWith(Application.dataPath))
                return path.Replace(Application.dataPath, "Assets");
            
            // If this absolute path isn't for this project, then return the top-level path
            else
                return "Assets";
        }
        
        public static string ConvertProjectPathToAbsolutePath(string path)
        {
            // If the project path is valid, replace "Assets" with application data path
            if (path.StartsWith("Assets"))
                return path.Replace("Assets", Application.dataPath);
            
            // If the project path is not valid, return an empty string
            else
                return "";
        }
        
        // ---------------------------------------------------------------------------
        // Root folder of the tool
        // ---------------------------------------------------------------------------
        // Returns the asset path of the folder that holds the tool resources, for example
        // "Packages/com.chnnasn.unity-3d-tilemap/Editor" or "Assets/TileMap3D".
        public static string GetToolRoot()
        {
            if (!string.IsNullOrEmpty(cachedToolRoot))
                return cachedToolRoot;

            // Preferred: locate the assembly definition that belongs to this tool.
            // Works for embedded, registry and git (URL) packages alike, and for a
            // plain folder inside the Assets directory.
            cachedToolRoot = FindToolRootFromAssemblyDefinition();

            // Fallback: scan the Assets folder for the tool folder
            if (string.IsNullOrEmpty(cachedToolRoot))
                cachedToolRoot = FindToolFolderInAssets();

            return cachedToolRoot;
        }

        // ---------------------------------------------------------------------------
        // Locate the tool by its assembly definition file
        // ---------------------------------------------------------------------------
        private static string FindToolRootFromAssemblyDefinition()
        {
            string assemblyName = typeof(LoadingHelper).Assembly.GetName().Name;
            string path = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
            if (string.IsNullOrEmpty(path) || Path.GetFileName(path) != "TileMap3D.Editor.asmdef")
                return "";

            // The asmdef is at <resource root>/Scripts/TileMap3D.Editor.asmdef.
            var root = Path.GetDirectoryName(Path.GetDirectoryName(path));
            return string.IsNullOrEmpty(root) ? "" : root.Replace('\\', '/');
        }

        // ---------------------------------------------------------------------------
        // Legacy name kept for source compatibility with older call sites
        // ---------------------------------------------------------------------------
        public static string GetMASTRootFolder()
        {
            return GetToolRoot();
        }

        // ---------------------------------------------------------------------------
        // Scan the Assets folder for a folder that contains the tool resources
        // ---------------------------------------------------------------------------
        private static string FindToolFolderInAssets()
        {
            foreach (string folder in Directory.EnumerateDirectories("Assets", "*", SearchOption.AllDirectories))
            {
                string folderName = Path.GetFileName(folder);
                if ((folderName == assetFolderName || folderName == legacyFolderName)
                    && File.Exists(folder + guiPath))
                    return folder.Replace('\\', '/');
            }

            return "";
        }

        // ---------------------------------------------------------------------------
        // Create a folder (and every missing parent folder) inside the project
        // ---------------------------------------------------------------------------
        public static void EnsureProjectFolder(string assetFolderPath)
        {
            if (string.IsNullOrEmpty(assetFolderPath))
                return;

            string normalized = assetFolderPath.Replace("\\", "/").TrimEnd('/');
            string[] parts = normalized.Split('/');

            if (parts.Length == 0 || parts[0] != "Assets")
                return;

            string current = "Assets";

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];

                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);

                current = next;
            }
        }

        // ---------------------------------------------------------------------------
        // Copy a template asset from the package into the project, if it is missing
        // ---------------------------------------------------------------------------
        public static T LoadOrCreateProjectAsset<T>(string projectPath, string templatePath) where T : ScriptableObject
        {
            EnsureProjectFolder(Path.GetDirectoryName(projectPath).Replace("\\", "/"));

            T asset = AssetDatabase.LoadAssetAtPath<T>(projectPath);

            if (asset == null && !string.IsNullOrEmpty(templatePath))
            {
                T template = AssetDatabase.LoadAssetAtPath<T>(templatePath);

                if (template != null)
                {
                    AssetDatabase.CopyAsset(templatePath, projectPath);
                    asset = AssetDatabase.LoadAssetAtPath<T>(projectPath);
                }
            }

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                // Save the newly created asset so the user does not lose their changes
                AssetDatabase.CreateAsset(asset, projectPath);
            }

            return asset;
        }

        // ---------------------------------------------------------------------------
        // Path of the folder holding the per project settings
        // ---------------------------------------------------------------------------
        public static string GetProjectSettingsFolder()
        {
            return MAST.Settings.Data.core != null
                ? MAST.Settings.Data.core.projectSettingsFolder
                : MAST.Const.Settings.defaultProjectSettingsFolder;
        }
        
        public static GameObject GetThumbnailCamera()
        {
            return (GameObject)AssetDatabase.LoadAssetAtPath(
                GetToolRoot() + thumbnailCameraPath, typeof(GameObject));
        }
        
        public static Texture2D GetImage(string fileName)
        {
            return (Texture2D)AssetDatabase.LoadAssetAtPath(
                GetToolRoot() + imagePath + fileName, typeof(Texture2D));
        }
        
        public static GUISkin GetGUISkin()
        {
            return (GUISkin)AssetDatabase.LoadAssetAtPath(
                GetToolRoot() + guiPath, typeof(GUISkin));
        }
        
        public static Material GetGridMaterial()
        {
            return (Material)AssetDatabase.LoadAssetAtPath(GetToolRoot()
                + gridMaterialPath, typeof(Material));
        }
        
        public static Material GetPaintAreaMaterial()
        {
            return (Material)AssetDatabase.LoadAssetAtPath(GetMASTRootFolder()
                + paintAreaMaterialPath, typeof(Material));
        }
        
        public static GameObject GetEraserPrefab()
        {
            return (GameObject)AssetDatabase.LoadAssetAtPath(
                GetMASTRootFolder() + eraserPrefabPath, typeof(GameObject));
        }
        
        // ---------------------------------------------------------------------------
        #region Get GameObjects from Selected Folder
        // ---------------------------------------------------------------------------
        public static GameObject[] GetPrefabsInFolder(string prefabFolder)
        {
            // Get the GUID of every file in that folder that is of the file type prefab
            string[] GUIDOfAllPrefabsInFolder =
                AssetDatabase.FindAssets("t:prefab", new[] { prefabFolder });
            
            // Create array to store the gameObjects
            GameObject[] allPrefabsAtPath = new GameObject[GUIDOfAllPrefabsInFolder.Length];
            
            // Create the string outside the loop so it is not recreated every loop
            string pathToPrefab;
            
            for (int index = GUIDOfAllPrefabsInFolder.Length - 1; index >= 0; index--)
            {
                // Convert GUID at current index to path string
                pathToPrefab = AssetDatabase.GUIDToAssetPath(GUIDOfAllPrefabsInFolder[index]);
                
                // Get gameObject at path
                allPrefabsAtPath[index] = (GameObject)AssetDatabase.LoadAssetAtPath(pathToPrefab, typeof(GameObject));
            }
            
            return allPrefabsAtPath;
        }
        
        // ---------------------------------------------------------------------------
        // Used by GetAllGameObjectsInSelectedFolder
        // ---------------------------------------------------------------------------
        public static string GetPathOfSelectedFolder()
        {
            // This code was entirely taken from https://gist.github.com/allanolivei/9260107
            string path = "Assets";
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            
            return path;
        }
        #endregion
        // ---------------------------------------------------------------------------
        
        // ---------------------------------------------------------------------------
        // Get path of the selected object
        // ---------------------------------------------------------------------------
        public static string GetPathOfSelectedObjectTypeOf(Type type)
        {
            // Get selected objects as object array
            UnityEngine.Object[] objs = Selection.GetFiltered(type, SelectionMode.Assets);
            
            // If correct object type was selected, return the first selected item's path
            if (objs.Length > 0)
                return AssetDatabase.GetAssetPath(objs[0]);
            
            // If no object of this type was selected, return an empty string
            else
                return "";
        }
        
        // ---------------------------------------------------------------------------
        // Choose a folder dialog
        // ---------------------------------------------------------------------------
        public static void Show_Choose_Folder_Dialog(string dialogTitle, string defaultPath)
        {
            string path = EditorUtility.OpenFolderPanel(dialogTitle, defaultPath, "");
        }
    }
}
#endif
