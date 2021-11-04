using System.Collections.Generic;
using UnityEngine;

namespace EunomiaUnity
{
    public static class ShaderExtensions
    {
        public static IEnumerable<string> GetProperties(this Shader shader, UnityEngine.Rendering.ShaderPropertyType type)
        {
            if (shader == null || shader.GetPropertyCount() == 0)
            {
                return null;
            }
            var result = new List<string>();
            for (var index = 0; index < shader.GetPropertyCount(); index++)
            {
                if (shader.GetPropertyType(index) == type)
                {
                    result.Add(shader.GetPropertyName(index));
                }
            }
            return result;
        }
    }
}