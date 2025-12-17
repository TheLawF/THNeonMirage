using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace THNeonMirage.Util
{
    public delegate int ActionStatement<out T>(Action<T> action);

    public delegate int LogStatement(string info);

    public static class Utils
    {
        public static string NextRandomString(int length, Range unicodeRange)
        {
            var sb = new StringBuilder();
            var list = new List<string>(unicodeRange.End.Value - unicodeRange.Start.Value);
            var stack = Shuffle(list.ConvertAll(input => Regex.Unescape($"\\u{list.IndexOf(input) + unicodeRange.Start.Value}")), length);
            for (var i = 0; i < stack.Count; i++) sb.Append(stack.Pop());
            return sb.ToString();
        }
        
        public static void WriteBool(MemoryStream stream, bool b)
        {
            var boolBytes = BitConverter.GetBytes(b);
            stream.Write(boolBytes, 0, boolBytes.Length);
        }
        public static void WriteInt(MemoryStream stream, int i)
        {
            var intBytes = BitConverter.GetBytes(i);
            stream.Write(intBytes, 0, intBytes.Length);
        }
        
        public static void WriteString(MemoryStream stream, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                stream.Write(BitConverter.GetBytes(0), 0, 4);
            }
            else
            {
                var stringBytes = System.Text.Encoding.UTF8.GetBytes(str);
                stream.Write(BitConverter.GetBytes(stringBytes.Length), 0, 4);
                stream.Write(stringBytes, 0, stringBytes.Length);
            }
        }
        
        public static GameObject FindDontDestroyedObj(string objName)
        {
            var ddScene = SceneManager.GetSceneByName("DontDestroyOnLoad");
            if (!ddScene.IsValid()) return null;
            var rootObjects = ddScene.GetRootGameObjects();
            return rootObjects.FirstOrDefault(obj => obj.name.Equals(objName));
        }
        public static string GetAddr(object o)
        {
            var h = GCHandle.Alloc(o, GCHandleType.WeakTrackResurrection);
            var addr = GCHandle.ToIntPtr(h);
            return "0x" + addr.ToString("X");
        }
        
        public static void ForAct(int count, Action<int> action)
        {
            for (var index = 0; index < count; index++) action.Invoke(index);
        }

        public static T ForFunc<T>(int count, T returnValue, Action<int, T> action)
        {
            for (var index = 0; index < count; index++) action.Invoke(index, returnValue);
            return returnValue;
        }
        
        public static object GetFieldValue(object obj, string fieldName)
            => obj.GetType().GetField(fieldName).GetValue(obj);
        
        public static T GetFieldValueAndCast<T>(object obj, string fieldName) => (T) GetFieldValue(obj, fieldName);

        public static List<T> CastJsonAsList<T>(string json, string fieldName) 
            => JObject.Parse(json)[fieldName]?.ToObject<List<T>>();

        public static int CastJsonAsInt(JObject jo, string fieldName) 
            => jo[fieldName].Type == JTokenType.Integer ? (int)jo[fieldName] : 0;

        public static string ListToString(ICollection list)
        {
            var sb = new StringBuilder();
            sb.Append('[');
            foreach (var each in list)
            {
                sb.Append(each);
                sb.Append(list.GetEnumerator().MoveNext() ? "," : "");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(']');
            return sb.ToString();
        }

        public static string ListToJsonString(string key, ICollection list)
        {
            var sb = new StringBuilder();
            sb.Append($"\"{key}\":");
            sb.Append(ListToString(list));
            return sb.ToString();
        }
        
        /// <summary>
        /// 将物体实例化之后循环添加到列表
        /// </summary>
        /// <param name="count">你想要添加到列表的元素个数</param>
        /// <param name="listToAdd">准备添加的列表</param>
        /// <param name="indexToObjFunc">使用索引值实例化物体对象的函数</param>
        public static void ForAddToList(int count, List<GameObject> listToAdd, Func<int, GameObject> indexToObjFunc)
            => ForAct(count, i => listToAdd.Add(indexToObjFunc.Invoke(i)));
        
        public static void ForAddToList<T>(int count, List<T> listToAdd, Func<int, T> indexToObjFunc)
            => ForAct(count, i => listToAdd.Add(indexToObjFunc.Invoke(i)));
        
        public static List<T> ForAddToListAndReturn<T>(int count, List<T> listToAdd, Func<int, T> indexToObjFunc)
            => ForFunc(count, listToAdd, (i, list) => list.Add(indexToObjFunc.Invoke(i)));

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

        public static Stack<int> Shuffle(int min, int max, int count) {
            if (count <= 0 || max <= min || count > max - min + 1)
                throw new ArgumentException("参数无效。");

            var range = Enumerable.Range(min, max - min + 1).ToList();
            var rng = new Random();
    
            // Fisher-Yates洗牌
            for (var i = range.Count - 1; i > 0; i--) {
                var j = rng.Next(i + 1);
                (range[j], range[i]) = (range[i], range[j]);
            }
    
            return new Stack<int>(range.Take(count).ToList());
        }

        public static Stack<T> Shuffle<T>(List<T> list, int takeCount)
        {
            var rng = new Random();
    
            // Fisher-Yates洗牌
            for (var i = list.Count - 1; i > 0; i--) {
                var j = rng.Next(i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
    
            return new Stack<T>(list.Take(takeCount).ToList());
        }

        public static T Roll<T>(List<T> list)
        {
            return list[new Random().Next(list.Count - 1)];
        }

    }
    
}