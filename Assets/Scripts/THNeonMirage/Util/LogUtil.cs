using System;
using UnityEngine;

namespace THNeonMirage.Util
{
    public delegate int LogStatement(string info);

    public delegate int ActionStatement<out T>(Action<T> action);
    public static class LogUtil
    {
        public static LogStatement OfInfo(string info) => str =>
        {
            Debug.Log(info);
            return 1;
        };
        
        public static LogStatement OfWarn(string info) => str =>
        {
            Debug.LogWarning(info);
            return 0;
        };
        public static LogStatement OfError(string info) => str =>
        {
            Debug.LogError(info);
            return -1;
        };

        public static ActionStatement<T> OfAction<T>(Action<T> action, T obj) => act =>
        {
            action.Invoke(obj);
            return 1;
        };

        public static int Info(string info) => OfInfo(info).Invoke(info);
        public static int Warn(string warn) => OfWarn(warn).Invoke(warn);
        public static int Error(string error) => OfError(error).Invoke(error);
        public static int Act<T>(Action<T> action, T parameter) => OfAction(action, parameter).Invoke(action);
    }
}