using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    [SerializeField]
    private Material scrollMaterialTexture;

    [SerializeField]
    private Renderer offsetRenderer;

    [SerializeField]
    private bool x = true;
    [SerializeField]
    private bool y = true;
    [SerializeField]
    private float offsetPerSecond = 0.1f;

    void Start()
    {
    }

    private Vector4 CurrentTextureCoordinates
    {
        get
        {
            if (scrollMaterialTexture != null)
            {
                return new Vector4(
                    scrollMaterialTexture.mainTextureScale.x,
                    scrollMaterialTexture.mainTextureScale.y,
                    scrollMaterialTexture.mainTextureOffset.x,
                    scrollMaterialTexture.mainTextureOffset.y
                );
            }
            else if (offsetRenderer != null)
            {
                var block = new MaterialPropertyBlock();
                offsetRenderer.GetPropertyBlock(block);
                return block.GetVector("_BaseMap_ST");
            }
            else
            {
                return new Vector4(1, 1, 0, 0);
            }
        }
        set
        {
            if (scrollMaterialTexture != null)
            {
                scrollMaterialTexture.mainTextureScale = new Vector2(value.x, value.y);
                scrollMaterialTexture.mainTextureOffset = new Vector2(value.z, value.w);
            }
            if (offsetRenderer != null)
            {
                var offset = new MaterialPropertyBlock();
                offsetRenderer.GetPropertyBlock(offset);
                var old = offset.GetVector("_BaseMap_ST");
                // TODO: get original tiling
                offset.SetVector("_BaseMap_ST", new Vector4(
                    value.x,
                    value.y,
                    value.z,
                    value.w)
                );
                offsetRenderer.SetPropertyBlock(offset);
            }
        }
    }

    void Update()
    {
        if (x || y)
        {
            CurrentTextureCoordinates = CurrentTextureCoordinates + new Vector4(
                0,
                0,
                x ? offsetPerSecond * Time.deltaTime : 0,
                y ? offsetPerSecond * Time.deltaTime : 0
            );
        }
    }
}
