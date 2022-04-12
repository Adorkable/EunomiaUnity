using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeFloatParameterStateMachineBehaviour : StateMachineBehaviour
{
    [SerializeField]
    public bool PerformOnStateEnter = false;
    [SerializeField]
    public bool PerformOnStateExit = false;

    [SerializeField]
    private string parameterName = "random";
    private int parameterNameHash;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!PerformOnStateEnter)
        {
            return;
        }
        CacheNameHash();
        RandomizeFloatParameterStateMachineBehaviour.SetRandom(animator, parameterNameHash);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!PerformOnStateExit)
        {
            return;
        }
        CacheNameHash();
        RandomizeFloatParameterStateMachineBehaviour.SetRandom(animator, parameterNameHash);
    }

    void CacheNameHash()
    {
        if (parameterNameHash == 0)
        {
            parameterNameHash = Animator.StringToHash(parameterName);
        }
    }

    private static void SetRandom(Animator animator, int randomParameterNameHash)
    {
        animator.SetFloat(randomParameterNameHash, UnityEngine.Random.value);
    }
}