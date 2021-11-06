using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public class SkyboxRotator : MonoBehaviour
    {
        // TODO: pull on Awake, report if not valid
        private static readonly int RotationShaderProperty = Shader.PropertyToID("_Rotation");
        [SerializeField] private float amountPerSecond = 0.1f;

        private float initialRotation;
        private float previousRotation;

        private void Awake()
        {
            initialRotation = RenderSettings.skybox.GetFloat(RotationShaderProperty);
            previousRotation = initialRotation;
        }

        private void Update()
        {
            // TODO: mod here so we don't have to pass the buck every render
            previousRotation += Time.deltaTime * amountPerSecond;
            RenderSettings.skybox.SetFloat(RotationShaderProperty, previousRotation);
        }

        private void OnDestroy()
        {
            // Last value gets written to material file, reseting so we don't change it
            RenderSettings.skybox.SetFloat(RotationShaderProperty, initialRotation);
        }
    }
}