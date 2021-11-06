using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class ListBoundsExtensions
    {
        public static List<RaycastHit> Raycast(this List<Bounds> boundsList,
            [DefaultValue("DefaultRaycastLayers")] int layerMask, Color drawRaysColor,
            float drawRaysDuration = 0)
        {
            return boundsList
                .ConvertAll((bounds) =>
                {
                    // TODO: HACK: padding around ray, decide on proper way
                    var rayStart = bounds.min - new Vector3(0.01f, 0.01f, 0.01f);
                    var cornerToCorner = bounds.max + new Vector3(0.01f, 0.01f, 0.01f) - rayStart;

                    if (drawRaysDuration > 0)
                    {
                        Debug.DrawRay(rayStart, cornerToCorner, drawRaysColor, drawRaysDuration);
                    }

                    Physics.Raycast(
                        rayStart,
                        cornerToCorner,
                        out var hitInfo,
                        cornerToCorner.magnitude,
                        layerMask
                    );

                    return hitInfo;
                })
                .FindAll((raycastHit) => raycastHit.collider != null);
        }

        public static List<RaycastHit> Raycast(this List<Bounds> boundsList,
            [DefaultValue("DefaultRaycastLayers")] int layerMask, Random random,
            float drawRaysDuration = 0)
        {
            return Raycast(boundsList, layerMask, random.ColorRGB(), drawRaysDuration);
        }

        public static List<RaycastHit> Raycast(this List<Bounds> boundsList,
            [DefaultValue("DefaultRaycastLayers")] int layerMask)
        {
            return boundsList
                .ConvertAll((bounds) =>
                {
                    // TODO: HACK: padding around ray, decide on proper way
                    var rayStart = bounds.min - new Vector3(0.01f, 0.01f, 0.01f);
                    var cornerToCorner = bounds.max + new Vector3(0.01f, 0.01f, 0.01f) - rayStart;

                    Physics.Raycast(
                        rayStart,
                        cornerToCorner,
                        out var hitInfo,
                        cornerToCorner.magnitude,
                        layerMask
                    );

                    return hitInfo;
                })
                .FindAll((raycastHit) => raycastHit.collider != null);
        }
    }
}