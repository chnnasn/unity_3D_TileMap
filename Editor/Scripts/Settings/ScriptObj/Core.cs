using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Settings
    {
        namespace ScriptObj
        {
            public class Core : ScriptableObject
            {
                // Folder inside the project that holds the per project settings of this tool.
                // Everything in here is writable by the user and must never live inside the package.
                [SerializeField] public string projectSettingsFolder =
                    MAST.Const.Settings.defaultProjectSettingsFolder;

                [SerializeField] public string guiSettingsPath = "";
                [SerializeField] public string placementSettingsPath = "";
                [SerializeField] public string hotkeySettingsPath = "";
                [SerializeField] public string interfaceStatePath = "";

                // ---------------------------------------------------------------------------
                // Fill in any path that is still empty
                // ---------------------------------------------------------------------------
                public void EnsurePaths()
                {
                    if (string.IsNullOrEmpty(projectSettingsFolder))
                        projectSettingsFolder = MAST.Const.Settings.defaultProjectSettingsFolder;

                    if (string.IsNullOrEmpty(guiSettingsPath))
                        guiSettingsPath = projectSettingsFolder + "/GUI.asset";

                    if (string.IsNullOrEmpty(placementSettingsPath))
                        placementSettingsPath = projectSettingsFolder + "/Placement.asset";

                    if (string.IsNullOrEmpty(hotkeySettingsPath))
                        hotkeySettingsPath = projectSettingsFolder + "/Hotkey.asset";

                    if (string.IsNullOrEmpty(interfaceStatePath))
                        interfaceStatePath = projectSettingsFolder + "/InterfaceState.asset";
                }
            }
        }
    }
}
#endif
