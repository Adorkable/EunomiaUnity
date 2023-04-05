using System.Collections;
using System.Collections.Generic;
using Eunomia;
using EunomiaUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof (Renderer))]
public class VideoFader : MonoBehaviour
{
    [SerializeField]
    public float fadeInDuration = 0.5f;

    [SerializeField]
    public float fadeOutDuration = 0.5f;

    [SerializeField, Dropdown("GetFadeMaterialParameterOptions")]
    private string fadeMaterialParameter;

    [SerializeField]
    private float fadeInAlpha = 1.0f;

    [SerializeField]
    private float fadeOutAlpha = 0.0f;

    enum FadeMode
    {
        None,
        In,
        Out
    }

    private FadeMode fadeMode = FadeMode.None;

    private float fadeStart;

    void Awake()
    {
        if (enabled == false)
        {
            return;
        }

        var rendererComponent = this.RequireComponentInstance<Renderer>();

        if (rendererComponent == null)
        {
            enabled = false;
            return;
        }
        else
        {
            rendererComponent.enabled = true;
        }
    }

    public void StartFadeOut()
    {
        fadeMode = FadeMode.Out;
        fadeStart = Time.time;
    }

    public void StartFadeIn()
    {
        fadeMode = FadeMode.In;
        fadeStart = Time.time;
    }

    void Update()
    {
        UpdateFade(); // TODO: elaborate insight in Notifications to be able to use it for fading in and fading out
    }

    void OnEnable()
    {
        var rendererComponent = gameObject.GetComponent<Renderer>();
        if (rendererComponent != null)
        {
            rendererComponent.enabled = true;
        }
    }

    void OnDisable()
    {
        var rendererComponent = gameObject.GetComponent<Renderer>();
        if (rendererComponent != null)
        {
            rendererComponent.enabled = false;
        }
    }

    private DropdownList<string> GetFadeMaterialParameterOptions()
    {
        var rendererComponent = this.GetComponent<Renderer>();
        if (rendererComponent == null)
        {
            return null;
        }

        var result = new DropdownList<string>();

        rendererComponent
            .GetSharedMaterialShaderProperties(ShaderPropertyType.Color)
            .ForEach((parameterName) =>
                result.Add(parameterName, parameterName));

        return result;
    }

    private void UpdateFade()
    {
        switch (fadeMode)
        {
            case FadeMode.Out:
                FadeOut();
                break;
            case FadeMode.In:
                FadeIn();
                break;
            case FadeMode.None:
                break;
        }
    }

    private void FadeOut()
    {
        if (fadeOutDuration <= 0)
        {
            fadeMode = FadeMode.None;
            return;
        }

        var rendererComponent = gameObject.GetComponent<Renderer>();
        if (rendererComponent == null || rendererComponent.material == null)
        {
            return; // TODO: throw?
        }

        var color = rendererComponent.material.GetColor(fadeMaterialParameter);

        float newAlpha;
        if (Time.time - fadeStart <= fadeOutDuration)
        {
            newAlpha =
                (Time.time - fadeStart)
                    .Map(0, fadeOutDuration, fadeInAlpha, fadeOutAlpha);
        }
        else
        {
            newAlpha = fadeOutAlpha;
            fadeMode = FadeMode.None;
        }

        if (newAlpha == color.a)
        {
            return;
        }

        var newColor = new Color(color.r, color.g, color.b, newAlpha);
        rendererComponent.material.SetColor (fadeMaterialParameter, newColor);
    }

    private void FadeIn()
    {
        if (fadeInDuration <= 0)
        {
            fadeMode = FadeMode.None;
            return;
        }

        var rendererComponent = gameObject.GetComponent<Renderer>();
        if (rendererComponent == null || rendererComponent.material == null)
        {
            return; // TODO: throw?
        }

        var color = rendererComponent.material.GetColor(fadeMaterialParameter);

        float newAlpha;
        if (Time.time - fadeStart <= fadeInDuration)
        {
            newAlpha =
                (Time.time - fadeStart)
                    .Map(0, fadeInDuration, fadeOutAlpha, fadeInAlpha);
        }
        else
        {
            newAlpha = fadeInAlpha;
            fadeMode = FadeMode.None;
        }

        if (newAlpha == color.a)
        {
            return;
        }

        var newColor = new Color(color.r, color.g, color.b, newAlpha);
        rendererComponent.material.SetColor (fadeMaterialParameter, newColor);
    }
}
