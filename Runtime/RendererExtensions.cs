using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class RendererExtensions
    {
        public static IEnumerable<string> GetSharedMaterialShaderProperties(this Renderer renderer,
            ShaderPropertyType type)
        {
            if (renderer == null || renderer.sharedMaterial == null)
            {
                return null;
            }

            return renderer.sharedMaterial.shader.GetProperties(type);
        }
    }
}