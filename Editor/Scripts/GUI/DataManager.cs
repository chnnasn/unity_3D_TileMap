using System.IO;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace GUI
    {
        public static class DataManager
        {
            public static MAST.GUI.ScriptObj.State state;
            
            // ---------------------------------------------------------------------------
            // Called during Interface OnEnable
            // ---------------------------------------------------------------------------
            public static void Initialize()
            {
                Get_Reference_To_Scriptable_Object();
            }
            
            // ---------------------------------------------------------------------------
            // Get or create the state scriptable object
            // ---------------------------------------------------------------------------
            private static void Get_Reference_To_Scriptable_Object()
            {
                // The interface state lives in the project, seeded by the package template on first run
                string statePath = MAST.Settings.Data.core != null && !string.IsNullOrEmpty(MAST.Settings.Data.core.interfaceStatePath)
                    ? MAST.Settings.Data.core.interfaceStatePath
                    : MAST.Const.Settings.defaultProjectSettingsFolder + "/InterfaceState.asset";

                string stateTemplatePath = MAST.LoadingHelper.GetToolRoot() + MAST.Const.Settings.stateTemplatePath;

                // Load the interface state scriptable object
                state = MAST.LoadingHelper.LoadOrCreateProjectAsset<MAST.GUI.ScriptObj.State>(
                    statePath, stateTemplatePath);
            }
            
            // ---------------------------------------------------------------------------
            // Save preferences to state scriptable object
            // ---------------------------------------------------------------------------
            
            public static void Save_Palette_Items(bool forceSave = false)
            {
                // Get or create a scriptable object to store the interface state data
                Get_Reference_To_Scriptable_Object();
                
                state.selectedPrefabPaletteFolderIndex = MAST.Building.Palette.Manager.selectedFolderIndex;
                
                Save_Changes_To_Disk();
            }
            
            public static void Restore_Palette_Items()
            {
                // Get or create a scriptable object to store the interface state data
                Get_Reference_To_Scriptable_Object();
                
                MAST.Building.Palette.Manager.LoadPrefabs(MAST.LoadingHelper.ConvertProjectPathToAbsolutePath(state.prefabPath),
                    state.selectedPrefabPaletteFolderIndex);
                
                MAST.Painting.Palette.Manager.LoadMaterials(MAST.LoadingHelper.ConvertProjectPathToAbsolutePath(state.materialPath),
                    state.selectedMaterialPaletteFolderIndex);
                
            }
            
            // ---------------------------------------------------------------------------
            // Save grid state preferences to state scriptable object
            // ---------------------------------------------------------------------------
            public static void Save_Interface_State()
            {
                // Get or create a scriptable object to store the interface state data
                Get_Reference_To_Scriptable_Object();
                
                // Save grid exists state
                state.gridExists = MAST.Building.GridManager.gridExists;
                
                // Save selected draw tool and palette
                state.selectedBuildToolIndex = MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex;
                state.selectedPrefabIndex = MAST.Building.Palette.Manager.selectedItemIndex;
                
                // Save state changes to disk
                Save_Changes_To_Disk();
            }
            
            // ---------------------------------------------------------------------------
            // Load grid state preferences from state scriptable object
            // ---------------------------------------------------------------------------
            public static void Load_Interface_State()
            {
                // Get or scriptable object to store the interface state data
                Get_Reference_To_Scriptable_Object();
                
                // -----------------------------------------------
                // If there is no saved scriptable object
                // -----------------------------------------------
                if (state == null)
                {
                    // Set grid exists to false
                    MAST.Building.GridManager.gridExists = false;
                    
                    // Make sure no palette item and build tool is selected
                    MAST.Building.GUI.Palette.RemovePrefabSelection();
                }
                
                // -----------------------------------------------
                // If there is a scriptable object
                // -----------------------------------------------
                else
                {
                    // Load grid exists state
                    MAST.Building.GridManager.gridExists = state.gridExists;
                    
                    // Load selected draw tool and palette
                    MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = state.selectedBuildToolIndex;
                    MAST.Building.Palette.Manager.selectedItemIndex = state.selectedPrefabIndex;
                    //MAST_Prefab_Palette_GUI.ChangePrefabSelection(state.selectedPrefabIndex);
                }
            }
            
            public static void Save_Changes_To_Disk()
            {
                // Save scriptable object changes
                // AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();
                EditorUtility.SetDirty(state);
            }
        }
    }
}
#endif