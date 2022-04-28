using Eunomia;
using EunomiaUnity;
using UnityEngine;

public class RandomInstantiator : AutoProcessor
{
    private bool hasStarted;
    override public bool HasStarted => hasStarted;
    private bool hasFinished;
    override public bool HasFinished => hasFinished;

    [SerializeField]
    private GameObject[] templates;
    [SerializeField]
    private bool matchPosition = false;
    [SerializeField]
    private bool matchRotation = false;
    [SerializeField]
    private bool matchScale = false;

    protected override void AutoProcess()
    {
        hasStarted = true;

        var template = templates.RandomElement();

        var instance = Instantiate(template);

        if (matchPosition)
        {
            instance.transform.position = transform.position;
        }
        if (matchRotation)
        {
            instance.transform.rotation = transform.rotation;
        }
        if (matchScale)
        {
            instance.transform.localScale = transform.localScale;
        }

        instance.transform.parent = transform;

        hasFinished = true;
    }
}
