using UnityEngine;

namespace EunomiaUnity
{
    public class SetBoolParameterStateMachineBehaviour : StateMachineBehaviour
    {
        [SerializeField]
        public bool PerformOnStateEnter = false;
        [SerializeField]
        public bool PerformOnStateExit = false;

        [SerializeField]
        private string parameterName = "bool_parameter";
        private int parameterNameHash;

        [SerializeField]
        public bool value = true;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!PerformOnStateEnter)
            {
                return;
            }
            CacheNameHash();
            SetBoolParameterStateMachineBehaviour.SetBool(animator, parameterNameHash, value);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!PerformOnStateExit)
            {
                return;
            }
            CacheNameHash();
            SetBoolParameterStateMachineBehaviour.SetBool(animator, parameterNameHash, value);
        }

        void CacheNameHash()
        {
            if (parameterNameHash == 0)
            {
                parameterNameHash = Animator.StringToHash(parameterName);
            }
        }

        private static void SetBool(Animator animator, int parameterName, bool value)
        {
            animator.SetBool(parameterName, value);
        }
    }
}