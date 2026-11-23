#if (UNITY_EDITOR)

namespace MAST
{
    public static class Const
    {
        // Grid
        public static class Grid
        {
            public static string defaultName = "MAST_Grid";
            public static string defaultParentName = "MAST_Grid_Parent";
            public static float yOffsetToAvoidTearing = -0.001f;
            public static int gridLayer = 4;
        }
        //public static Grid_Class grid = new Grid_Class();

        // Settings
        public static class Settings
        {
            // Folder inside the project where the per project settings of the tool are stored
            public static string defaultProjectSettingsFolder = "Assets/TileMap3D/Settings";

            // Templates shipped inside the package, used to seed the project settings on first run
            public static string coreTemplatePath = "/Etc/CoreSettings.asset";
            public static string stateTemplatePath = "/Etc/InterfaceState.asset";
            public static string guiTemplatePath = "/Settings/GUI.asset";
            public static string placementTemplatePath = "/Settings/Placement.asset";
            public static string hotkeyTemplatePath = "/Settings/Hotkey.asset";
        }
        
        // Placement
        public static class Placement
        {
            public static string defaultTargetParentName = "MAST_Holder";
            public static string defaultTargetParentTag = "MAST_Holder";
            public static int visualizerLayer = 2;
        }
        //public static Placement_Class placement = new Placement_Class();
        
        
    }
}
#endif