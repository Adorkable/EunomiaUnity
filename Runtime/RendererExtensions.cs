using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        // TODO: add animation support
        /**
        * Sets the color of a renderer, using insight into the type of Renderer for special cases.
        *
        * @param renderer The renderer to set the color of.
        * @param color The color to set.
        */
        public static void FlexibleSetColor(this Renderer renderer, Color color)
        {
            if (renderer is SpriteRenderer)
            {
                var spriteRenderer = (renderer as SpriteRenderer);
                spriteRenderer.color = color;
            }
            else if (renderer is TrailRenderer)
            {
                var trailRenderer = (renderer as TrailRenderer);
                trailRenderer.startColor = color;
                trailRenderer.endColor = color;
            }
            else if (renderer is LineRenderer)
            {
                var lineRenderer = (renderer as LineRenderer);
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;
            }
            else
            {
                renderer.SetToMaterialPropertyBlock(color);
            }
        }

        // TODO: optionally test fields available for type
        static void SetToMaterialPropertyBlock(this Renderer renderer, Action<MaterialPropertyBlock, string> setValue, IEnumerable<string> propertyNames, bool maintainOriginalBlock = true, bool onlyFirstProperty = false)
        {
            if (propertyNames == null || propertyNames.Count() == 0)
            {
                // TODO: throw
                return;
            }

            var block = new MaterialPropertyBlock();
            if (maintainOriginalBlock)
            {
                renderer.GetPropertyBlock(block);
            }
            for (var index = 0; index < propertyNames.Count(); index++)
            {
                var propertyName = propertyNames.ElementAt(index);

                setValue(block, propertyName);

                if (onlyFirstProperty)
                {
                    break;
                }
            }
            renderer.SetPropertyBlock(block);
        }

        private static IEnumerable<string> UsePropertyNames(this Renderer renderer, ShaderPropertyType type, IEnumerable<string> onlyPropertyNames = null)
        {
            IEnumerable<string> result = null;
            if (onlyPropertyNames != null)
            {
                result = onlyPropertyNames;
            }
            else
            {
                result = renderer.sharedMaterial.GetShaderProperties(type);
            }

            if (result == null || result.Count() == 0)
            {
                throw new DataException($"Material {renderer.sharedMaterial} Shader does not contain a {type} property");
            }
            return result;
        }

        public static void SetToMaterialPropertyBlock(this Renderer renderer, Texture2D texture, bool maintainOriginalBlock = true, bool onlyFirstProperty = false, IEnumerable<string> onlyPropertyNames = null)
        {
            IEnumerable<string> propertyNames = UsePropertyNames(renderer, ShaderPropertyType.Texture, onlyPropertyNames);

            SetToMaterialPropertyBlock(renderer, (block, propertyName) => block.SetTexture(propertyName, texture), propertyNames, maintainOriginalBlock, onlyFirstProperty);
        }

        public static void SetToMaterialPropertyBlock(this Renderer renderer, Color color, bool maintainOriginalBlock = true, bool onlyFirstProperty = false, IEnumerable<string> onlyPropertyNames = null)
        {
            IEnumerable<string> propertyNames = UsePropertyNames(renderer, ShaderPropertyType.Color, onlyPropertyNames);

            SetToMaterialPropertyBlock(renderer, (block, propertyName) => block.SetColor(propertyName, color), propertyNames, maintainOriginalBlock, onlyFirstProperty);
        }

        public enum ColorOperation
        {
            Assign,
            Multiply
        }

        public static void SetToMaterialPropertyBlock(this Renderer renderer, Color color, ColorOperation operation, bool maintainOriginalBlock = true, bool onlyFirstProperty = false, IEnumerable<string> onlyPropertyNames = null)
        {
            IEnumerable<string> propertyNames = UsePropertyNames(renderer, ShaderPropertyType.Color, onlyPropertyNames);

            SetToMaterialPropertyBlock(renderer, (block, propertyName) =>
            {
                Color computedValue;
                switch (operation)
                {
                    case ColorOperation.Assign:
                        computedValue = color;
                        break;
                    case ColorOperation.Multiply:
                        {
                            var currentValue = renderer.sharedMaterial.GetColor(propertyName);
                            computedValue = currentValue * color;
                        }
                        break;
                    default:
                        computedValue = color;
                        break;
                }
                block.SetColor(propertyName, computedValue);
            }, propertyNames, maintainOriginalBlock, onlyFirstProperty);
        }
    }
}