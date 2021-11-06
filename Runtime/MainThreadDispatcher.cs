using System;
using Eunomia;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    // Based on: https://www.lucidumstudio.com/home/2017/12/5/lucidum-studio-async-code-execution-in-unity
    public class MainThreadDispatcher : MonoBehaviour
    {
        private Dispatcher mainThreadDispatcher;

        public Action<Exception> logUnhandledExceptions
        {
            get { return mainThreadDispatcher.logUnhandledExceptions; }
            set { mainThreadDispatcher.logUnhandledExceptions = value; }
        }

        protected void Awake()
        {
            mainThreadDispatcher = new Dispatcher();
        }

        private void Update()
        {
            mainThreadDispatcher.InvokePending();
        }

        public void Invoke(Action fn)
        {
            mainThreadDispatcher.Invoke(fn);
        }
    }
}