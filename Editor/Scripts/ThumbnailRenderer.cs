using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MAST
{
    internal sealed class ThumbnailRenderer : IDisposable
    {
        const int Size = 128;
        readonly PreviewRenderUtility preview;

        public ThumbnailRenderer(Camera template = null)
        {
            if (template == null)
            {
                var rig = LoadingHelper.GetThumbnailCamera();
                if (rig != null)
                    template = rig.GetComponentInChildren<Camera>(true);
            }

            preview = new PreviewRenderUtility();
            var camera = preview.camera;
            camera.fieldOfView = template != null ? template.fieldOfView : 27f;
            camera.transform.rotation = Quaternion.Euler(Settings.Data.gui.palette.snapshotCameraYaw,
                Settings.Data.gui.palette.snapshotCameraPitch, 0);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0.25f);
            preview.ambientColor = Color.gray;
            preview.lights[0].intensity = 1.2f;
            preview.lights[0].transform.rotation = camera.transform.rotation * Quaternion.Euler(25, -35, 0);
            preview.lights[1].intensity = 0.6f;
        }

        public Texture2D Render(GameObject prefab)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(prefab))
                return RenderAndDestroy(preview.InstantiatePrefabInScene(prefab));

            var copy = Object.Instantiate(prefab);
            preview.AddSingleGO(copy);
            return RenderAndDestroy(copy);
        }

        public Texture2D Render(Material material)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            preview.AddSingleGO(sphere);
            sphere.GetComponent<Renderer>().sharedMaterial = material;
            return RenderAndDestroy(sphere);
        }

        Texture2D RenderAndDestroy(GameObject subject)
        {
            try
            {
                var renderers = subject.GetComponentsInChildren<Renderer>();
                var bounds = renderers.Length == 0 ? new Bounds(subject.transform.position, Vector3.one) : renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    bounds.Encapsulate(renderers[i].bounds);

                var camera = preview.camera;
                float radius = Mathf.Max(bounds.extents.magnitude, 0.01f);
                float distance = radius / Mathf.Sin(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                camera.transform.position = bounds.center - camera.transform.forward * distance;
                camera.nearClipPlane = Mathf.Max(0.001f, distance - 1.1f * radius);
                camera.farClipPlane = distance + 1.1f * radius;
                return Capture();
            }
            finally
            {
                Object.DestroyImmediate(subject);
            }
        }

        Texture2D Capture()
        {
            var rect = new Rect(0, 0, Size, Size);
            preview.BeginPreview(rect, null);
            Texture rendered;
            try
            {
                preview.Render(true);
            }
            finally
            {
                rendered = preview.EndPreview();
            }

            var previous = RenderTexture.active;
            var target = RenderTexture.GetTemporary(Size, Size, 0, RenderTextureFormat.ARGB32);
            Texture2D image = null;
            try
            {
                Graphics.Blit(rendered, target);
                RenderTexture.active = target;
                image = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
                image.ReadPixels(rect, 0, 0);
                image.Apply();
                return image;
            }
            catch
            {
                if (image != null)
                    Object.DestroyImmediate(image);
                throw;
            }
            finally
            {
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(target);
            }
        }

        public static void SavePng(Texture2D image, string path)
        {
            try
            {
                File.WriteAllBytes(path, image.EncodeToPNG());
            }
            finally
            {
                Object.DestroyImmediate(image);
            }
        }

        public void Dispose()
        {
            preview.Cleanup();
        }
    }
}
