using System;
using System.IO;
using UnityEngine;

namespace EunomiaUnity
{
    public static class RenderTextureExtensions_WriteToDisk
    {
        // TODO: autodetecting encoder from extension WriteToDisk function

        public static void WriteToDisk(this RenderTexture renderTexture, string fileName, Func<Texture2D, byte[]> encoder, TextureFormat fileTextureFormat = TextureFormat.RGBA32)
        {
            var frame = new Texture2D(renderTexture.width, renderTexture.height, fileTextureFormat, false);

            var buffer = RenderTexture.active;
            RenderTexture.active = renderTexture;
            frame.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            RenderTexture.active = buffer;

            var bytes = encoder(frame);
            File.WriteAllBytes(fileName, bytes);
        }

        public static void WritePNGToDisk(this RenderTexture renderTexture, string fileName, TextureFormat fileTextureFormat = TextureFormat.RGBA32)
        {
            WriteToDisk(renderTexture, fileName, (Texture2D texture) => texture.EncodeToPNG(), fileTextureFormat);
        }

        public static string WritePNGToTemporaryDisk(this RenderTexture renderTexture, TextureFormat fileTextureFormat = TextureFormat.RGBA32)
        {
            var savePath = $"{Application.temporaryCachePath}/RenderTexture-{renderTexture.GetInstanceID()}_{DateTime.Now.ToFileTimeUtc()}.png";
            WritePNGToDisk(renderTexture, savePath, fileTextureFormat);
            return savePath;
        }

        public static void WriteTGAToDisk(this RenderTexture renderTexture, string fileName, TextureFormat fileTextureFormat = TextureFormat.RGBA32)
        {
            WriteToDisk(renderTexture, fileName, (Texture2D texture) => texture.EncodeToTGA(), fileTextureFormat);

        }

        public static string WriteTGAToTemporaryDisk(this RenderTexture renderTexture, TextureFormat fileTextureFormat = TextureFormat.RGBA32)
        {
            var savePath = $"{Application.temporaryCachePath}/RenderTexture-{renderTexture.GetInstanceID()}_{DateTime.Now.ToFileTimeUtc()}.tga";
            WriteTGAToDisk(renderTexture, savePath, fileTextureFormat);
            return savePath;
        }

        public static void WriteJPGToDisk(this RenderTexture renderTexture, string fileName, TextureFormat fileTextureFormat = TextureFormat.RGB24)
        {
            WriteToDisk(renderTexture, fileName, (Texture2D texture) => texture.EncodeToJPG(), fileTextureFormat);

        }

        public static string WriteJPGToTemporaryDisk(this RenderTexture renderTexture, TextureFormat fileTextureFormat = TextureFormat.RGB24)
        {
            var savePath = $"{Application.temporaryCachePath}/RenderTexture-{renderTexture.GetInstanceID()}_{DateTime.Now.ToFileTimeUtc()}.jpg";
            WriteJPGToDisk(renderTexture, savePath, fileTextureFormat);
            return savePath;
        }
    }

    public static class RenderTextureExtensions_Clear
    {
        public static void Clear(this RenderTexture renderTexture)
        {
            Clear(renderTexture, Color.clear);
        }

        public static void Clear(this RenderTexture renderTexture, Color clearColor)
        {
            Clear(renderTexture, true, true, clearColor);
        }

        public static void Clear(this RenderTexture renderTexture, bool enableClearColor, bool enableClearDepth, Color clearColor, float clearDepth = 1.0f)
        {
            RenderTexture previous = RenderTexture.active;

            RenderTexture.active = renderTexture;
            GL.Clear(enableClearDepth, enableClearColor, clearColor, clearDepth);

            RenderTexture.active = previous;
        }
    }
}