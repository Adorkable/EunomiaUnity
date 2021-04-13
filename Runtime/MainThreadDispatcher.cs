using System;
using System.Collections.Generic;
using UnityEngine;

namespace EunomiaUnity
{
    // Based on: https://www.lucidumstudio.com/home/2017/12/5/lucidum-studio-async-code-execution-in-unity
    public class MainThreadDispatcher : MonoBehaviour
    {
        private Eunomia.MainThreadDispatcher mainThreadDispatcher;

        protected void Awake()
        {
            mainThreadDispatcher = new Eunomia.MainThreadDispatcher();
        }

        public void Invoke(Action fn)
        {
            mainThreadDispatcher.Invoke(fn);
        }

        private void Update()
        {
            mainThreadDispatcher.Update();
        }
    }
}
