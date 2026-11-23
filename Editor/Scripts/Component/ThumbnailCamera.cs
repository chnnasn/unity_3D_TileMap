using System;
using UnityEngine;

namespace MAST.Component
{
    [ExecuteInEditMode]
    public class ThumbnailCamera : MonoBehaviour
    {
        [SerializeField] private Camera snapshotCamera;

        public Texture2D[] GetPrefabThumbnails(GameObject[] prefabs)
        {
            using (var renderer = new ThumbnailRenderer(snapshotCamera))
                return Array.ConvertAll(prefabs, renderer.Render);
        }

        public Texture2D[] GetMaterialThumbnails(Material[] materials)
        {
            using (var renderer = new ThumbnailRenderer(snapshotCamera))
                return Array.ConvertAll(materials, renderer.Render);
        }
    }
}
