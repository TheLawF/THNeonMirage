using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Fictology.Event
{
    public class EventHelper
    {
        // 缓存所有发现的 EventKey 字段
    private static Dictionary<string, Dictionary<string, EventKey>> _eventKeyCache = new Dictionary<string, Dictionary<string, EventKey>>();
    private static bool _isCacheDirty = true;

    /// <summary>
    /// 获取所有程序集中的所有公开静态只读 EventKey 字段
    /// </summary>
    public static Dictionary<string, Dictionary<string, EventKey>> GetAllEventKeyFields()
    {
        if (!_isCacheDirty && _eventKeyCache.Count > 0)
            return _eventKeyCache;

        RefreshCache();
        return _eventKeyCache;
    }

    /// <summary>
    /// 刷新 EventKey 字段缓存
    /// </summary>
    public static void RefreshCache()
    {
        _eventKeyCache.Clear();
        
        // 获取所有已加载的程序集
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        foreach (var assembly in assemblies)
        {
            // 跳过系统程序集以提高性能
            if (ShouldSkipAssembly(assembly))
                continue;

            try
            {
                FindEventKeysInAssembly(assembly);
            }
            catch (Exception ex)
            {
                // Debug.LogWarning($"在程序集 {assembly.FullName} 中查找 EventKey 时出错: {ex.Message}");
            }
        }
        
        _isCacheDirty = false;
        // Debug.Log($"发现 {_eventKeyCache.Values.Sum(dict => dict.Count)} 个 EventKey 字段");
    }

    /// <summary>
    /// 在指定程序集中查找 EventKey 字段
    /// </summary>
    private static void FindEventKeysInAssembly(Assembly assembly)
    {
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // 处理类型加载异常
            types = ex.Types.Where(t => t != null).ToArray();
        }
        catch (Exception ex)
        {
            // Debug.LogWarning($"获取程序集 {assembly.FullName} 中的类型时出错: {ex.Message}");
            return;
        }

        foreach (var type in types)
        {
            if (type == null) continue;

            try
            {
                FindEventKeysInType(type);
            }
            catch (Exception ex)
            {
                // Debug.LogWarning($"在类型 {type.Name} 中查找 EventKey 时出错: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 在指定类型中查找 EventKey 字段
    /// </summary>
    private static void FindEventKeysInType(Type type)
    {
        // 获取所有公开的静态字段
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        
        foreach (var field in fields)
        {
            // 检查字段类型是否为 EventKey
            if (field.FieldType != typeof(EventKey))
                continue;

            // 检查字段是否为只读 (InitOnly)
            if (!field.IsInitOnly)
                continue;

            // 检查字段是否具有 public 和 static 修饰符
            if (!field.IsPublic || !field.IsStatic)
                continue;

            try
            {
                // 获取字段值
                var eventKey = field.GetValue(null) as EventKey;
                if (eventKey == null)
                {
                    // Debug.LogWarning($"字段 {type.Name}.{field.Name} 的值为 null");
                    continue;
                }

                // 添加到缓存
                string className = type.FullName;
                if (!_eventKeyCache.ContainsKey(className))
                {
                    _eventKeyCache[className] = new Dictionary<string, EventKey>();
                }

                _eventKeyCache[className][field.Name] = eventKey;
            }
            catch (Exception ex)
            {
                // Debug.LogWarning($"获取字段 {type.Name}.{field.Name} 的值时出错: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 判断是否应跳过该程序集
    /// </summary>
    private static bool ShouldSkipAssembly(Assembly assembly)
    {
        string assemblyName = assembly.FullName;
        
        // 跳过系统程序集
        if (assemblyName.StartsWith("System.") ||
            assemblyName.StartsWith("Microsoft.") ||
            assemblyName.StartsWith("Unity.") ||
            assemblyName.StartsWith("UnityEngine.") ||
            assemblyName.StartsWith("UnityEditor.") ||
            assemblyName.StartsWith("mscorlib") ||
            assemblyName.StartsWith("netstandard") ||
            assemblyName.StartsWith("Mono."))
        {
            return true;
        }

        // 跳过动态程序集
        if (assembly.IsDynamic)
            return true;

        return false;
    }

    /// <summary>
    /// 根据类名和字段名获取 EventKey
    /// </summary>
    public static EventKey GetEventKey(string className, string fieldName)
    {
        if (_eventKeyCache.TryGetValue(className, out var classFields))
        {
            if (classFields.TryGetValue(fieldName, out var eventKey))
            {
                return eventKey;
            }
        }

        // 如果缓存中没有，尝试实时查找
        return FindEventKeyRealTime(className, fieldName);
    }

    /// <summary>
    /// 实时查找 EventKey（不依赖缓存）
    /// </summary>
    private static EventKey FindEventKeyRealTime(string className, string fieldName)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        foreach (var assembly in assemblies)
        {
            if (ShouldSkipAssembly(assembly))
                continue;

            try
            {
                var type = assembly.GetType(className);
                if (type == null) continue;

                var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
                if (field != null && 
                    field.FieldType == typeof(EventKey) && 
                    field.IsInitOnly)
                {
                    return field.GetValue(null) as EventKey;
                }
            }
            catch (Exception ex)
            {
                // Debug.LogWarning($"实时查找 EventKey {className}.{fieldName} 时出错: {ex.Message}");
            }
        }

        return null;
    }

    /// <summary>
    /// 获取所有 EventKey 的显示名称
    /// </summary>
    public static Dictionary<string, string> GetEventKeyDisplayNames()
    {
        var result = new Dictionary<string, string>();
        
        foreach (var classPair in _eventKeyCache)
        {
            foreach (var fieldPair in classPair.Value)
            {
                if (fieldPair.Value != null)
                {
                    // 使用完整类名作为键
                    var key = $"{classPair.Key}.{fieldPair.Key}";
                    // 显示名称：EventKey值 (类名.字段名)
                    var displayName = $"{fieldPair.Value} ({GetShortClassName(classPair.Key)}.{fieldPair.Key})";
                    result[key] = displayName;
                }
            }
        }
        
        return result;
    }

    /// <summary>
    /// 获取短类名（去掉命名空间）
    /// </summary>
    private static string GetShortClassName(string fullClassName)
    {
        int lastDotIndex = fullClassName.LastIndexOf('.');
        return lastDotIndex >= 0 ? fullClassName.Substring(lastDotIndex + 1) : fullClassName;
    }

    /// <summary>
    /// 标记缓存为脏，下次访问时会刷新
    /// </summary>
    public static void MarkCacheDirty()
    {
        _isCacheDirty = true;
    }
    }
}