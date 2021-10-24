using UnityEngine;

namespace EunomiaUnity
{
    public static class ApplicationExtensions
    {
        public static string executablePath
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + "/../";
#elif UNITY_STANDALONE_WIN
            return Application.dataPath + "/../";
#elif UNITY_STANDALONE_OSX
            return Application.dataPath + "/../../";
#endif
            }
        }
    }
}