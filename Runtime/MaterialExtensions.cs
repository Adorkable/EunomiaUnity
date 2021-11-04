using System.Collections.Generic;
using UnityEngine;

namespace EunomiaUnity
{
    public static class MaterialExtensions
    {
        public static IEnumerable<string> GetShaderProperties(this Material material, UnityEngine.Rendering.ShaderPropertyType type)
        {
            if (material == null || material.shader == null)
            {
                return null;
            }

            return material.shader.GetProperties(type);
        }
    }
}