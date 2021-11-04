using System.Collections.Generic;
using UnityEngine;

namespace EunomiaUnity
{
    public static class RendererExtensions
    {
        public static IEnumerable<string> GetSharedMaterialShaderProperties(this Renderer renderer, UnityEngine.Rendering.ShaderPropertyType type)
        {
            if (renderer == null || renderer.sharedMaterial == null)
            {
                return null;
            }

            return renderer.sharedMaterial.shader.GetProperties(type);
        }
    }
}