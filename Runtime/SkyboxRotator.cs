using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [SerializeField]
    private float amountPerSecond = 0.1f;

    private float initialRotation;
    private float previousRotation;

    void Awake()
    {
        initialRotation = RenderSettings.skybox.GetFloat("_Rotation");
        previousRotation = initialRotation;
    }

    void OnDestroy()
    {
        // Last value gets written to material file, reseting so we don't change it
        RenderSettings.skybox.SetFloat("_Rotation", initialRotation);
    }

    void Update()
    {
        // TODO: mod here so we don't have to pass the buck every render
        previousRotation += Time.deltaTime * amountPerSecond;
        RenderSettings.skybox.SetFloat("_Rotation", previousRotation);
    }
}