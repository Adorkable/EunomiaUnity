using UnityEngine;

namespace EunomiaUnity
{
    /// <summary> Extension methods for <see cref="Texture2D"/>. </summary>
    public static class Texture2DExtensions
    {
        /// <summary> Get the average color of a <see cref="Texture2D"/>. </summary>
        /// <param name="texture"> The <see cref="Texture2D"/> to get the average color of. </param>
        /// <returns> The average color of the <see cref="Texture2D"/>. </returns>
        public static Color GetAverageColor(this Texture2D texture)
        {
            var count = 0;
            var r = 0.0f;
            var g = 0.0f;
            var b = 0.0f;
            var a = 0.0f;
            for (var x = 0; x < texture.width; x++)
            {
                for (var y = 0; y < texture.height; y++)
                {
                    var color = texture.GetPixel(x, y);
                    r += color.r;
                    g += color.g;
                    b += color.b;
                    a += color.a;

                    count += 1;
                }
            }
            return new Color(
                r / (float)(count),
                g / (float)(count),
                b / (float)(count),
                a / (float)(count)
            );
        }
    }
}