using System;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Building
    {
        [Serializable]
        public static class GridManager
        {
            // ---------------------------------------------------------------------------
            #region Variable Declaration
            // ---------------------------------------------------------------------------
            
            // Grid Appearance
            [SerializeField] public static bool gridExists = false;
            
            // Grid in Scene
            [SerializeField] private static GameObject gridGameObject;
            [SerializeField] private static Material gridMaterial;
            //[SerializeField] private static GameObject gridParent; // hidden in inspector with child grid left visible so it still draws gizmolines
            
            #endregion
            // ---------------------------------------------------------------------------
            
            // ---------------------------------------------------------------------------
            // Initialize
            // ---------------------------------------------------------------------------
            public static void Initialize()
            {
                
            }
            
            // ---------------------------------------------------------------------------
            #region Grid Location
            // ---------------------------------------------------------------------------
            public static void MoveGridUp()
            {
                if (gridExists)
                {
                    // Move Grid Up
                    Settings.Data.gui.grid.gridHeight += 1;
                    MoveGridToNewHeight();
                }
            }
            
            public static void MoveGridDown()
            {
                if (gridExists)
                {
                    // Move Grid Up
                    Settings.Data.gui.grid.gridHeight -= 1;
                    MoveGridToNewHeight();
                }
            }
            
            private static void MoveGridToNewHeight()
            {
                // Delegate to MoveGridToNewPosition which handles both XZ offset and Y height
                MoveGridToNewPosition();
            }
            
            // ---------------------------------------------------------------------------
            // Grid Offset (moves the entire grid plane to a new world position)
            // ---------------------------------------------------------------------------
            public static void SetGridOffset(float offsetX, float offsetZ)
            {
                Settings.Data.gui.grid.gridOffsetX = offsetX;
                Settings.Data.gui.grid.gridOffsetZ = offsetZ;
                MoveGridToNewPosition();
            }
            
            public static void MoveGridOffsetX(float delta)
            {
                Settings.Data.gui.grid.gridOffsetX += delta;
                MoveGridToNewPosition();
            }
            
            public static void MoveGridOffsetZ(float delta)
            {
                Settings.Data.gui.grid.gridOffsetZ += delta;
                MoveGridToNewPosition();
            }
            
            private static void MoveGridToNewPosition()
            {
                if (gridGameObject != null)
                {
                    // Move entire grid plane to new world XZ position, keeping current Y height
                    float gridY = Settings.Data.gui.grid.gridHeight * Settings.Data.gui.grid.yUnitSize
                        + Settings.Data.gui.grid.gridOffsetY + Const.Grid.yOffsetToAvoidTearing;
                    gridGameObject.transform.position =
                        new Vector3(Settings.Data.gui.grid.gridOffsetX, gridY, Settings.Data.gui.grid.gridOffsetZ);
                }
            }
            #endregion
            // ---------------------------------------------------------------------------
            
            // ---------------------------------------------------------------------------
            #region Create/Destroy Grid
            // ---------------------------------------------------------------------------
            
            // Return if grid reference exists
            public static bool DoesGridExist()
            {
                return gridExists;
            }
            
            // Change grid visibility
            public static void ChangeGridVisibility()
            {
                if (gridExists)
                {
                    CreateGrid();
                }
                else
                {
                    DestroyGrid();
                    
                    // Deselect draw tool and palette item and destroy any active prefab visualizer
                    GUI.Palette.RemovePrefabSelection();
                }
            }
            
            // Destroy any existing grid
            public static void DestroyGrid()
            {
                // Find existing grid(s) and delete them - even if disabled
                foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (go.name == Const.Grid.defaultName)
                    {
                        GameObject.DestroyImmediate(go);
                    }
                }

                // Remove locked layer
                UnityEditor.Tools.lockedLayers &= ~(1 << Const.Grid.gridLayer);
                
                gridExists = false;
            }
            
            // Create grid
            public static void CreateGrid()
            {
                DestroyGrid();
                if (gridMaterial == null)
                    gridMaterial = LoadingHelper.GetGridMaterial();
                if (gridMaterial == null)
                {
                    Debug.LogError("3D TileMap could not load the grid material at " +
                        LoadingHelper.GetToolRoot() + "/Etc/Material_Grid.mat");
                    return;
                }

                CreateNewGrid();

                // Lock the layer the grid is on
                UnityEditor.Tools.lockedLayers |= 1 << Const.Grid.gridLayer;
                
                gridExists = true;
            }
            
            // ---------------------------------------------------------------------------
            // Create a New Grid in the Hierarchy from the Grid Prefab
            // ---------------------------------------------------------------------------
            static void CreateNewGrid()
            {
                // Create new Grid GameObject at the saved offset position
                gridGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                gridGameObject.transform.position =
                    new Vector3(Settings.Data.gui.grid.gridOffsetX, 0f, Settings.Data.gui.grid.gridOffsetZ);
                gridGameObject.name = Const.Grid.defaultName;
                gridGameObject.layer = Const.Grid.gridLayer;
                
                // Configure Grid GameObject MeshRenderer
                MeshRenderer gridMeshRenderer = gridGameObject.GetComponent<MeshRenderer>();
                gridMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                gridMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                gridMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                gridMeshRenderer.receiveShadows = false;
                
                // Apply the grid material, tiling and tint to the renderer.
                UpdateGridSettings();
                
                // Return the grid to its last saved height
                MoveGridToNewHeight();
                
                // Hide the grid in the hierarchy
                gridGameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            #endregion
            // ---------------------------------------------------------------------------
            
            // ---------------------------------------------------------------------------
            #region Grid Settings
            // ---------------------------------------------------------------------------
            public static void UpdateGridSettings()
            {
                if (gridGameObject != null)
                {
                    // Scale plane to match grid area (default Unity Plane = 10x10 world units)
                    gridGameObject.transform.localScale =
                        new Vector3(Settings.Data.gui.grid.gridSizeX / 10f,
                        1f,
                        Settings.Data.gui.grid.gridSizeZ / 10f);
                    
                    MoveGridToNewPosition();

                    // Override appearance on the scene grid; the package material is read-only.
                    MeshRenderer gridMeshRenderer = gridGameObject.GetComponent<MeshRenderer>();
                    gridMeshRenderer.sharedMaterial = gridMaterial;
                    var properties = new MaterialPropertyBlock();
                    Vector2 offset = gridMaterial.GetTextureOffset("_GridTexture");
                    properties.SetVector("_GridTexture_ST", new Vector4(
                        Settings.Data.gui.grid.cellCountX / 2f, Settings.Data.gui.grid.cellCountZ / 2f,
                        offset.x, offset.y));
                    properties.SetColor("_Tint", Settings.Data.gui.grid.tintColor);
                    gridMeshRenderer.SetPropertyBlock(properties);
                }
            }
            #endregion
            // ---------------------------------------------------------------------------
        }
    }
}

#endif
