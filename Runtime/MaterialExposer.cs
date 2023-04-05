using System;
using System.Collections.Generic;
using System.Linq;
using EunomiaUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Renderer))]
public class MaterialExposer : MonoBehaviour
{
    private Renderer rendererComponent
    {
        get
        {
            return this.GetComponent<Renderer>();
        }
    }

    private List<string> GetMaterialColorParameterOptions()
    {
        var renderer = rendererComponent;
        if (renderer == null)
        {
            return null;
        }

        return renderer.GetSharedMaterialShaderProperties(ShaderPropertyType.Color).ToList();
    }

    [SerializeField, Dropdown("GetMaterialColorParameterOptions")]
    private string materialColorParameter;
    private int materialColorParameterId;

    [SerializeField, Dropdown("GetMaterialColorParameterOptions")]
    private string materialEmissiveColorParameter;
    private int materialEmissiveColorParameterId;

    void Awake()
    {
        materialColorParameterId = Shader.PropertyToID(materialColorParameter);
        materialEmissiveColorParameterId = Shader.PropertyToID(materialEmissiveColorParameter);
    }

    // TODO: animated version
    public void SetMaterialColor(Color color)
    {
        var renderer = rendererComponent;
        if (renderer == null)
        {
            return;
        }

        renderer.material.SetColor(materialColorParameterId, color);
    }

    public void SetMaterialColor(string hexColor)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hexColor, out color);
        SetMaterialColor(color);
    }

    // TODO: animated version
    public void SetMaterialEmissiveColor(Color color)
    {
        var renderer = rendererComponent;
        if (renderer == null)
        {
            return;
        }

        renderer.material.SetColor(materialEmissiveColorParameterId, color);
    }

    public void SetMaterialEmissiveColor(string hexColor)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hexColor, out color);
        SetMaterialEmissiveColor(color);
    }

    ///////////

    // TODO: animated version
    public void SetMaterialColor(string parameterId, string hexColor)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hexColor, out color);
        SetMaterialColor(parameterId, color);
    }

    public void SetMaterialColor(string parameterId, Color color)
    {
        var renderer = rendererComponent;
        if (renderer == null)
        {
            return;
        }

        renderer.material.SetColor(parameterId, color);
    }

    [Serializable]
    public struct MaterialColor
    {
        public string parameterId;
        public string hexColor;
    };
    public void SetMaterialColor(MaterialColor materialColor)
    {
        SetMaterialColor(materialColor.parameterId, materialColor.hexColor);
    }

}
