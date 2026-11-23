using System;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Settings
    {
        namespace ScriptObj
        {
            [Serializable]
            public class GUI : ScriptableObject
            {
                [SerializeField] public Grid grid;
                [Serializable] public class Grid
                {
                    [SerializeField] public float cellSizeX = 1f;     // fixed cell size in world units (X)
                    [SerializeField] public float cellSizeZ = 1f;     // fixed cell size in world units (Z)
                    [SerializeField] public float yUnitSize = 1f;
                    
                    // cells from center to each edge (total = 2 * cellCount)
                    [SerializeField] public int cellCountX = 50;
                    [SerializeField] public int cellCountZ = 50;
                    
                    [SerializeField] public int gridHeight = 0;
                    
                    [SerializeField] public Color tintColor = new Color32(255, 255, 255, 127);
                    
                    [SerializeField] public float gridOffsetX = 0f;
                    [SerializeField] public float gridOffsetZ = 0f;
                    [SerializeField] public float gridOffsetY = 0f;
                    
                    // Derived: total grid area in world units (center to edge * 2)
                    public float gridSizeX { get { return 2 * cellCountX * cellSizeX; } }
                    public float gridSizeZ { get { return 2 * cellCountZ * cellSizeZ; } }
                    
                    [SerializeField] public float yPos()
                    {
                        return gridHeight * yUnitSize;
                    }
                }
                
                [SerializeField] public Palette palette;
                [Serializable] public class Palette
                {
                    [SerializeField] public PaleteBGColor bgColor = PaleteBGColor.Dark;
                    [SerializeField] public float snapshotCameraPitch = 225f;
                    [SerializeField] public float snapshotCameraYaw = 30f;
                    [SerializeField] public bool overwriteThumbnails = false;
                }
                
                [SerializeField] public Toolbar toolbar;
                [Serializable] public class Toolbar
                {
                    [SerializeField] public int selectedDrawToolIndex = -1;
                    [SerializeField] public int selectedPaintToolIndex = -1;
                    [SerializeField] public ToolbarPos position = ToolbarPos.Left;
                    [SerializeField] public float scale = 1f;
                }
            }
        }
    }
}
#endif