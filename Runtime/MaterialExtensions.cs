using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class MaterialExtensions
    {
        public static IEnumerable<string> GetShaderProperties(this Material material,
            ShaderPropertyType type)
        {
            if (material == null || material.shader == null)
            {
                return null;
            }

            return material.shader.GetProperties(type);
        }
    }
}