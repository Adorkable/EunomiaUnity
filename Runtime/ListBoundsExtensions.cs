using System.Collections.Generic;
using UnityEngine;

namespace EunomiaUnity {
    public static class ListBoundsExtensions {
        public static List<RaycastHit> Raycast(this List<Bounds> boundsList, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, float drawRaysDuration = 0) {
            return boundsList.ConvertAll((bounds) => {
                // TODO: HACK: padding around ray, decide on proper way
                var rayStart = bounds.min - new Vector3(0.01f, 0.01f, 0.01f);
                var cornerToCorner = (bounds.max + new Vector3(0.01f, 0.01f, 0.01f)) - rayStart;

                if (drawRaysDuration > 0) {
                    Debug.DrawRay(rayStart, cornerToCorner, EunomiaUnity.Random.ColorRGB, drawRaysDuration);
                }

                RaycastHit hitInfo;
                Physics.Raycast(
                    rayStart,
                    cornerToCorner,
                    out hitInfo,
                    cornerToCorner.magnitude,
                    layerMask
                );

                return hitInfo;
            }).FindAll((raycastHit) => {
                return raycastHit.collider != null;
            });
        }
    }
}