using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MAST.Tests
{
    public class PaletteThumbnailTests
    {
        [Test]
        public void LoadingFolderGeneratesUsablePrefabAndMaterialThumbnails()
        {
            var previousSettings = Settings.Data.gui;
            var settings = ScriptableObject.CreateInstance<Settings.ScriptObj.GUI>();
            settings.palette = new Settings.ScriptObj.GUI.Palette();
            Settings.Data.gui = settings;
            var guid = AssetDatabase.CreateFolder("Assets", "TileMapThumbnailTest_" + Guid.NewGuid().ToString("N"));
            var folder = AssetDatabase.GUIDToAssetPath(guid);
            try
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                try
                {
                    PrefabUtility.SaveAsPrefabAsset(cube, folder + "/Cube.prefab");
                }
                finally
                {
                    Object.DestroyImmediate(cube);
                }
                var material = new Material(Shader.Find("Unlit/Color"));
                material.color = Color.red;
                AssetDatabase.CreateAsset(material, folder + "/Red.mat");

                var path = LoadingHelper.ConvertProjectPathToAbsolutePath(folder);
                new Building.Palette.IO().Initialize(path, true, false);
                new Painting.Palette.IO().Initialize(path, true, false);
                AssertThumbnail(folder + "/_MAST_Thumbnails/Cube.png");
                AssertThumbnail(folder + "/_MAST_Thumbnails/Red.png");
            }
            finally
            {
                Settings.Data.gui = previousSettings;
                Object.DestroyImmediate(settings);
                AssetDatabase.DeleteAsset(folder);
            }
        }

        static void AssertThumbnail(string path)
        {
            var imported = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Assert.That(imported, Is.Not.Null, path);
            Assert.That(imported.width, Is.EqualTo(128));
            Assert.That(imported.height, Is.EqualTo(128));
            var decoded = new Texture2D(2, 2);
            try
            {
                Assert.That(decoded.LoadImage(File.ReadAllBytes(path)), Is.True);
                var pixels = decoded.GetPixels32();
                Assert.That(pixels.Any(pixel => !pixel.Equals(pixels[0])), Is.True, "Expected a visible subject in " + path);
            }
            finally
            {
                Object.DestroyImmediate(decoded);
            }
        }
    }
}
