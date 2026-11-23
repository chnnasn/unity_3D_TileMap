using System;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Settings
    {
        [Serializable]
        public static class Data
        { 
            [SerializeField] public static MAST.Settings.ScriptObj.Core core;
            [SerializeField] public static MAST.Settings.ScriptObj.GUI gui;
            [SerializeField] public static MAST.Settings.ScriptObj.Placement placement;
            [SerializeField] public static MAST.Settings.ScriptObj.Hotkey hotkey;
            
            // ---------------------------------------------------------------------------
            // When class is enabled
            // ---------------------------------------------------------------------------
            public static void Initialize()
            {
                // Load settings from scriptable object if it was lost
                if (core == null)
                    Load_Settings();
            }
            
            static void OnDisable() {}
            
            static void OnFocus() {}
            
            static void OnDestroy() {}
            
            // ---------------------------------------------------------------------------
            #region Manage Settings Scriptable Object
            // ---------------------------------------------------------------------------
            public static void Load_Settings()
            {
                Load_Core_Settings();
                Load_GUI_Settings();
                Load_Placement_Settings();
                Load_Hotkey_Settings();
                
                // Save the scriptable object
                //AssetDatabase.SaveAssets();
            }
            
            public static void Load_Core_Settings()
            {
                // The core settings live in the project, so the user can change them
                // without touching the (read only) package content.
                string projectCorePath = MAST.Const.Settings.defaultProjectSettingsFolder + "/CoreSettings.asset";
                string coreTemplatePath = MAST.LoadingHelper.GetToolRoot() + MAST.Const.Settings.coreTemplatePath;

                // Load the core scriptable object, copying the package template on first run
                core = MAST.LoadingHelper.LoadOrCreateProjectAsset<MAST.Settings.ScriptObj.Core>(
                    projectCorePath, coreTemplatePath);

                // Make sure every path the tool relies on is filled in
                core.EnsurePaths();

                // Save the core settings in case they were just created
                EditorUtility.SetDirty(core);
            }

            public static void Load_GUI_Settings()
            {
                // Load the GUI scriptable object from the project, seeded by the package template
                gui = MAST.LoadingHelper.LoadOrCreateProjectAsset<MAST.Settings.ScriptObj.GUI>(
                    core.guiSettingsPath,
                    MAST.LoadingHelper.GetToolRoot() + MAST.Const.Settings.guiTemplatePath);
            }

            public static void Load_Placement_Settings()
            {
                // Load the Placement scriptable object from the project, seeded by the package template
                placement = MAST.LoadingHelper.LoadOrCreateProjectAsset<MAST.Settings.ScriptObj.Placement>(
                    core.placementSettingsPath,
                    MAST.LoadingHelper.GetToolRoot() + MAST.Const.Settings.placementTemplatePath);
            }

            public static void Load_Hotkey_Settings()
            {
                // Load the Hotkey scriptable object from the project, seeded by the package template
                hotkey = MAST.LoadingHelper.LoadOrCreateProjectAsset<MAST.Settings.ScriptObj.Hotkey>(
                    core.hotkeySettingsPath,
                    MAST.LoadingHelper.GetToolRoot() + MAST.Const.Settings.hotkeyTemplatePath);
            }
            
            // Save preferences to a scriptable object
            public static void Save_Settings()
            {
                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();
                
                EditorUtility.SetDirty(core);
                EditorUtility.SetDirty(gui);
                EditorUtility.SetDirty(placement);
                EditorUtility.SetDirty(hotkey);
            }
            
            #endregion
            // ---------------------------------------------------------------------------
        }
    }
}
#endif