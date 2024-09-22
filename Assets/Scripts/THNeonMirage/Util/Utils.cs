using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace THNeonMirage.Util
{
    public delegate int ActionStatement<out T>(Action<T> action);

    public delegate int LogStatement(string info);

    public static class Utils
    {
        public static void ForAct(int count, Action<int> action)
        {
            for (var index = 0; index < count; index++) action.Invoke(index);
        }

        /// <summary>
        /// 将物体实例化之后循环添加到列表
        /// </summary>
        /// <param name="count">你想要添加到列表的元素个数</param>
        /// <param name="listToAdd">准备添加的列表</param>
        /// <param name="indexToObjFunc">使用索引值实例化物体对象的函数</param>
        public static void ForAddToList(int count, List<GameObject> listToAdd, Func<int, GameObject> indexToObjFunc)
            => ForAct(count, i => listToAdd.Add(indexToObjFunc.Invoke(i)));

        /// <summary>
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="index"></param>
        /// <typeparam name="TReturn"></typeparam>
        /// <returns></returns>
        public static TReturn SwitchByMap<TReturn>(List<Func<TReturn>> functions, int index)
        {
            var obj = index switch
            {
                >= 0 and < 10 => functions[0].Invoke(),
                >= 10 and < 20 => functions[1].Invoke(),
                >= 20 and < 30 => functions[2].Invoke(),
                >= 30 and < 40 => functions[3].Invoke(),
                
                _ => functions[4].Invoke()
            };
            return obj;
        }
        
        public static TReturn SwitchByMap<TReturn, T1>(List<Func<T1, TReturn>> functions, int index, T1 t1)
        {
            var obj = index switch
            {
                >= 0 and < 10 => functions[0].Invoke(t1),
                >= 10 and < 20 => functions[1].Invoke(t1),
                >= 20 and < 30 => functions[2].Invoke(t1),
                >= 30 and < 40 => functions[3].Invoke(t1),
                
                _ => functions[4].Invoke(t1)
            };
            return obj;
        }
        
        public static TReturn SwitchFunc<TObj, TReturn>(List<Predicate<TObj>> conditions,
            List<Func<TReturn>> functions, TObj tObj)
        {
            var res = default(TReturn);
            conditions.ForEach(predicate =>
            {
                if (predicate.Invoke(tObj)) res = functions[conditions.IndexOf(predicate)].Invoke();
            });
            return res;
        }
        
        public static void SwitchAction<TObj, TParam>(List<Predicate<TObj>> conditions,
            List<Action<TParam>> actions, TObj tObj, TParam tParam)
        {
            conditions.ForEach(predicate =>
            {
                if (predicate.Invoke(tObj)) actions[conditions.IndexOf(predicate)].Invoke(tParam);
            });
        }

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

        public static bool IsInRange(Range range, int num) => num >= range.Start.Value && num < range.End.Value;
        
        public static int Info(string info) => OfInfo(info).Invoke(info);
        public static int Warn(string warn) => OfWarn(warn).Invoke(warn);
        public static int Error(string error) => OfError(error).Invoke(error);
        public static int Act<T>(Action<T> action, T parameter) => OfAction(action, parameter).Invoke(action);
    }
    
}