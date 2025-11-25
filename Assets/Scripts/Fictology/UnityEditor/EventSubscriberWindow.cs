using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Fictology.Event;
using FlyRabbit.EventCenter.Core;
using THNeonMirage.Registry;
using UnityEditor;
using UnityEngine;

namespace Fictology.UnityEditor
{
    public class EventSubscriberWindow: EditorWindow
    {
        [MenuItem("Fictology/Event Center/事件查看器")]
        public static void CreateWindow()
        {
            GetWindow<EventSubscriberWindow>("事件查看器");
        }

        // 存储分析结果：EventKey -> 引用列表
        private Dictionary<string, List<EventReference>> eventReferences;
        private Vector2 scrollPosition;
        private string selectedEventKey;
        private bool showListeners = true;
        private bool showTriggers = true;
        private string searchFilter = "";
        private bool groupByFile = false;
        private bool autoRefresh = true;
        
        // 正则表达式 - 匹配 EventCenter 调用
        private static readonly Regex m_AddRegex = new Regex(
            @"(?<!""[^\s]*)EventCenter\s*\.\s*AddListener\s*(?:<\s*(?<Types>[^>]+)\s*>)?\s*\(\s*(?<ClassName>\w+)\s*\.\s*(?<FieldName>\w+)",
            RegexOptions.Compiled | RegexOptions.Singleline);
            
        private static readonly Regex m_RemoveRegex = new Regex(
            @"(?<!""[^\s]*)EventCenter\s*\.\s*RemoveListener\s*(?:<\s*(?<Types>[^>]+)\s*>)?\s*\(\s*(?<ClassName>\w+)\s*\.\s*(?<FieldName>\w+)",
            RegexOptions.Compiled | RegexOptions.Singleline);
            
        private static readonly Regex m_TriggerRegex = new Regex(
            @"(?<!""[^\s]*)EventCenter\s*\.\s*TriggerEvent\s*(?:<\s*(?<Types>[^>]+)\s*>)?\s*\(\s*(?<ClassName>\w+)\s*\.\s*(?<FieldName>\w+)",
            RegexOptions.Compiled | RegexOptions.Singleline);

        // 缓存 EventKey 发现结果
        private Dictionary<string, Dictionary<string, EventKey>> eventKeyCache = new Dictionary<string, Dictionary<string, EventKey>>();
        private DateTime lastCacheRefreshTime = DateTime.MinValue;
        private static readonly TimeSpan CacheRefreshInterval = TimeSpan.FromSeconds(2);

        private void OnEnable()
        {
            RefreshEventKeyCache();
            if (autoRefresh)
            {
                RefreshAnalysis();
            }
            
            // 注册编译完成事件
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        private void OnAfterAssemblyReload()
        {
            if (autoRefresh)
            {
                RefreshEventKeyCache();
                RefreshAnalysis();
                Repaint();
            }
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawContent();
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("刷新分析", EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    RefreshAnalysis();
                }
                
                if (GUILayout.Button("刷新缓存", EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    RefreshEventKeyCache();
                }
                
                GUILayout.Space(10);
                
                showListeners = GUILayout.Toggle(showListeners, "监听", EditorStyles.toolbarButton, GUILayout.Width(60));
                showTriggers = GUILayout.Toggle(showTriggers, "触发", EditorStyles.toolbarButton, GUILayout.Width(60));
                groupByFile = GUILayout.Toggle(groupByFile, "按文件分组", EditorStyles.toolbarButton, GUILayout.Width(80));
                autoRefresh = GUILayout.Toggle(autoRefresh, "自动刷新", EditorStyles.toolbarButton, GUILayout.Width(80));
                
                GUILayout.FlexibleSpace();
                
                // 搜索框
                GUILayout.Label("搜索:", GUILayout.ExpandWidth(false));
                GUILayout.Space(5);
                
                string newSearchFilter = GUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(200));
                if (newSearchFilter != searchFilter)
                {
                    searchFilter = newSearchFilter;
                    Repaint();
                }
                
                if (!string.IsNullOrEmpty(searchFilter) && GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    searchFilter = "";
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();
            
            // 统计信息
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                int totalEvents = eventReferences?.Count ?? 0;
                int totalReferences = eventReferences?.Values.Sum(list => list.Count) ?? 0;
                int totalListeners = eventReferences?.Values.Sum(list => list.Count(r => r.type == EventReferenceType.AddListener || r.type == EventReferenceType.RemoveListener)) ?? 0;
                int totalTriggers = eventReferences?.Values.Sum(list => list.Count(r => r.type == EventReferenceType.TriggerEvent)) ?? 0;
                
                GUILayout.Label($"事件: {totalEvents} | 引用: {totalReferences} | 监听: {totalListeners} | 触发: {totalTriggers}", 
                    EditorStyles.miniLabel);
                
                GUILayout.FlexibleSpace();
                
                GUILayout.Label($"缓存更新: {lastCacheRefreshTime:HH:mm:ss}", EditorStyles.miniLabel);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawContent()
        {
            if (eventReferences == null)
            {
                EditorGUILayout.HelpBox("点击\"刷新分析\"按钮开始分析事件中心使用情况", MessageType.Info);
                return;
            }

            if (eventReferences.Count == 0)
            {
                EditorGUILayout.HelpBox("未发现任何 EventCenter 调用。请确保：\n1. 项目中有使用 EventCenter 的代码\n2. EventKey 字段是 public static readonly 的", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                var filteredEvents = GetFilteredEvents();
                
                foreach (var eventKey in filteredEvents)
                {
                    DrawEventSection(eventKey);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private IEnumerable<string> GetFilteredEvents()
        {
            var events = eventReferences.Keys.OrderBy(x => x).ToList();
            
            if (!string.IsNullOrEmpty(searchFilter))
            {
                events = events.Where(x => 
                    x.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    eventReferences[x].Any(r => 
                        r.className.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        r.fieldName.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0))
                    .OrderBy(x => x)
                    .ToList();
            }
            
            return events;
        }

        private void DrawEventSection(string eventKey)
        {
            var references = eventReferences[eventKey];
            var listenerCount = references.Count(r => r.type == EventReferenceType.AddListener || r.type == EventReferenceType.RemoveListener);
            var triggerCount = references.Count(r => r.type == EventReferenceType.TriggerEvent);

            // 获取 EventKey 的详细信息
            string displayName = GetEventKeyDisplayName(eventKey, references.FirstOrDefault());
            
            bool isExpanded = selectedEventKey == eventKey;
            bool newExpanded = EditorGUILayout.Foldout(isExpanded, 
                $"{displayName} (监听: {listenerCount}, 触发: {triggerCount}, 引用: {references.Count})", true);
            
            if (newExpanded != isExpanded)
            {
                selectedEventKey = newExpanded ? eventKey : null;
            }

            if (newExpanded)
            {
                EditorGUI.indentLevel++;
                DrawEventReferences(references);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
        }

        private string GetEventKeyDisplayName(string eventKey, EventReference reference)
        {
            if (reference != null)
            {
                return $"{eventKey} (定义于: {reference.className}.{reference.fieldName})";
            }
            return eventKey;
        }

        private void DrawEventReferences(List<EventReference> references)
        {
            if (groupByFile)
            {
                DrawReferencesGroupedByFile(references);
            }
            else
            {
                DrawReferencesGroupedByType(references);
            }
        }

        private void DrawReferencesGroupedByType(List<EventReference> references)
        {
            // 监听器部分
            if (showListeners)
            {
                var addListeners = references.Where(r => r.type == EventReferenceType.AddListener).ToList();
                if (addListeners.Count > 0)
                {
                    EditorGUILayout.LabelField("📞 添加监听器:", EditorStyles.boldLabel);
                    foreach (var reference in addListeners)
                    {
                        DrawReferenceItem(reference);
                    }
                    EditorGUILayout.Space();
                }

                var removeListeners = references.Where(r => r.type == EventReferenceType.RemoveListener).ToList();
                if (removeListeners.Count > 0)
                {
                    EditorGUILayout.LabelField("❌ 移除监听器:", EditorStyles.boldLabel);
                    foreach (var reference in removeListeners)
                    {
                        DrawReferenceItem(reference);
                    }
                    EditorGUILayout.Space();
                }
            }

            // 触发器部分
            if (showTriggers)
            {
                var triggers = references.Where(r => r.type == EventReferenceType.TriggerEvent).ToList();
                if (triggers.Count > 0)
                {
                    EditorGUILayout.LabelField("🎯 触发事件:", EditorStyles.boldLabel);
                    foreach (var reference in triggers)
                    {
                        DrawReferenceItem(reference);
                    }
                }
            }
        }

        private void DrawReferencesGroupedByFile(List<EventReference> references)
        {
            var referencesByFile = references.GroupBy(r => r.filePath)
                                            .OrderBy(g => g.Key);
            
            foreach (var fileGroup in referencesByFile)
            {
                string fileName = Path.GetFileName(fileGroup.Key);
                EditorGUILayout.LabelField($"📄 {fileName}", EditorStyles.boldLabel);
                
                foreach (var reference in fileGroup.OrderBy(r => r.lineNumber))
                {
                    if ((showListeners && (reference.type == EventReferenceType.AddListener || reference.type == EventReferenceType.RemoveListener)) ||
                        (showTriggers && reference.type == EventReferenceType.TriggerEvent))
                    {
                        DrawReferenceItem(reference);
                    }
                }
                EditorGUILayout.Space();
            }
        }

        private void DrawReferenceItem(EventReference reference)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // 类型图标和文本
                string typeText = reference.type switch
                {
                    EventReferenceType.AddListener => "📞 添加监听",
                    EventReferenceType.RemoveListener => "❌ 移除监听", 
                    EventReferenceType.TriggerEvent => "🎯 触发事件",
                    _ => "❓ 未知"
                };

                EditorGUILayout.LabelField(typeText, GUILayout.Width(100));
                
                // 泛型类型信息
                if (!string.IsNullOrEmpty(reference.genericTypes))
                {
                    EditorGUILayout.LabelField($"<{reference.genericTypes}>", GUILayout.Width(120));
                }
                else
                {
                    GUILayout.Space(120);
                }

                // 文件信息
                string fileName = Path.GetFileName(reference.filePath);
                EditorGUILayout.LabelField($"{fileName}:{reference.lineNumber}", GUILayout.Width(150));

                // 类和方法信息
                string callInfo = $"{reference.className}.{reference.fieldName}";
                EditorGUILayout.LabelField(callInfo, GUILayout.ExpandWidth(true));

                // 跳转按钮
                if (GUILayout.Button("跳转", GUILayout.Width(60)))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(reference.filePath, reference.lineNumber);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RefreshEventKeyCache()
        {
            try
            {
                eventKeyCache.Clear();
                
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                int processedAssemblies = 0;
                
                foreach (var assembly in assemblies)
                {
                    processedAssemblies++;
                    
                    if (ShouldSkipAssembly(assembly))
                        continue;

                    try
                    {
                        EditorUtility.DisplayProgressBar("刷新 EventKey 缓存", 
                            $"扫描程序集: {GetAssemblyDisplayName(assembly)}", 
                            (float)processedAssemblies / assemblies.Length);

                        FindEventKeysInAssembly(assembly);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"在程序集 {assembly.FullName} 中查找 EventKey 时出错: {ex.Message}");
                    }
                }
                
                lastCacheRefreshTime = DateTime.Now;
                Debug.Log($"发现 {eventKeyCache.Values.Sum(dict => dict.Count)} 个 EventKey 字段");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void FindEventKeysInAssembly(Assembly assembly)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"获取程序集 {assembly.FullName} 中的类型时出错: {ex.Message}");
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
                    Debug.LogWarning($"在类型 {type.Name} 中查找 EventKey 时出错: {ex.Message}");
                }
            }
        }

        private void FindEventKeysInType(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(EventKey) && field.IsInitOnly)
                {
                    try
                    {
                        var eventKey = field.GetValue(null) as EventKey;
                        if (eventKey != null)
                        {
                            string className = type.FullName;
                            if (!eventKeyCache.ContainsKey(className))
                            {
                                eventKeyCache[className] = new Dictionary<string, EventKey>();
                            }

                            eventKeyCache[className][field.Name] = eventKey;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"获取字段 {type.Name}.{field.Name} 的值时出错: {ex.Message}");
                    }
                }
            }
        }

        private bool ShouldSkipAssembly(Assembly assembly)
        {
            string assemblyName = assembly.FullName;
            
            return assemblyName.StartsWith("System.") ||
                   assemblyName.StartsWith("Microsoft.") ||
                   assemblyName.StartsWith("UnityEngine.") ||
                   assemblyName.StartsWith("UnityEditor.") ||
                   assemblyName.StartsWith("mscorlib") ||
                   assemblyName.StartsWith("netstandard") ||
                   assembly.IsDynamic;
        }

        private string GetAssemblyDisplayName(Assembly assembly)
        {
            var name = new AssemblyName(assembly.FullName);
            return name.Name;
        }

        private EventKey GetEventKey(string className, string fieldName)
        {
            if (DateTime.Now - lastCacheRefreshTime > CacheRefreshInterval)
            {
                RefreshEventKeyCache();
            }

            if (eventKeyCache.TryGetValue(className, out var classFields))
            {
                if (classFields.TryGetValue(fieldName, out var eventKey))
                {
                    return eventKey;
                }
            }

            // 尝试实时查找
            return FindEventKeyRealTime(className, fieldName);
        }

        private EventKey FindEventKeyRealTime(string className, string fieldName)
        {
            try
            {
                var type = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .FirstOrDefault(t => t.FullName == className || t.Name == className);
                
                if (type != null)
                {
                    var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
                    if (field != null && field.FieldType == typeof(EventKey) && field.IsInitOnly)
                    {
                        var eventKey = field.GetValue(null) as EventKey;
                        
                        // 更新缓存
                        if (!eventKeyCache.ContainsKey(className))
                        {
                            eventKeyCache[className] = new Dictionary<string, EventKey>();
                        }
                        eventKeyCache[className][fieldName] = eventKey;
                        
                        return eventKey;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"实时查找 EventKey 失败: {className}.{fieldName}, 错误: {ex.Message}");
            }
            
            return null;
        }

        private void RefreshAnalysis()
        {
            if (eventKeyCache.Count == 0)
            {
                RefreshEventKeyCache();
            }

            eventReferences = new Dictionary<string, List<EventReference>>();
            
            // 获取所有 C# 脚本文件
            string[] scriptGuids = AssetDatabase.FindAssets("t:Script");
            var scriptPaths = scriptGuids.Select(AssetDatabase.GUIDToAssetPath)
                                        .Where(path => path.StartsWith("Assets/") && path.EndsWith(".cs"))
                                        .ToArray();

            int processedFiles = 0;
            foreach (var filePath in scriptPaths)
            {
                processedFiles++;
                string fullPath = Application.dataPath + "/../" + filePath;
                
                EditorUtility.DisplayProgressBar("分析事件中心", 
                    $"正在分析 {Path.GetFileName(filePath)} ({processedFiles}/{scriptPaths.Length})", 
                    (float)processedFiles / scriptPaths.Length);

                try
                {
                    AnalyzeFile(fullPath, filePath);
                }
                catch (Exception ex)
                {
                    // Debug.LogError($"分析文件 {filePath} 时出错: {ex.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            Repaint();
            
            // Debug.Log($"分析完成: 发现 {eventReferences.Count} 个事件，共 {eventReferences.Values.Sum(list => list.Count)} 个引用");
        }

        private void AnalyzeFile(string fullPath, string assetPath)
        {
            if (!File.Exists(fullPath)) return;
            
            var content = File.ReadAllText(fullPath);
            content = RemoveComments(content);
            var lines = File.ReadAllLines(fullPath);

            AnalyzeWithRegex(content, lines, assetPath, m_AddRegex, EventReferenceType.AddListener);
            AnalyzeWithRegex(content, lines, assetPath, m_RemoveRegex, EventReferenceType.RemoveListener);
            AnalyzeWithRegex(content, lines, assetPath, m_TriggerRegex, EventReferenceType.TriggerEvent);
        }

        private void AnalyzeWithRegex(string content, string[] lines, string filePath, Regex regex, EventReferenceType referenceType)
        {
            var matches = regex.Matches(content);
            foreach (Match match in matches)
            {
                if (match.Groups["ClassName"].Success && match.Groups["FieldName"].Success)
                {
                    string className = match.Groups["ClassName"].Value;
                    string fieldName = match.Groups["FieldName"].Value;
                    string genericTypes = match.Groups["Types"].Success ? match.Groups["Types"].Value : "";
                    
                    // 通过反射获取 EventKey
                    var eventKey = GetEventKey(className, fieldName);
                    
                    if (eventKey != null)
                    {
                        string eventKeyString = eventKey.ToString();
                        
                        // 计算行号
                        int lineNumber = GetLineNumber(content, match.Index, lines) + 1;

                        var reference = new EventReference
                        {
                            type = referenceType,
                            eventKeyFieldName = eventKeyString,
                            className = className,
                            fieldName = fieldName,
                            filePath = filePath,
                            lineNumber = lineNumber,
                            genericTypes = genericTypes
                        };

                        if (!eventReferences.ContainsKey(eventKeyString))
                        {
                            eventReferences[eventKeyString] = new List<EventReference>();
                        }

                        eventReferences[eventKeyString].Add(reference);
                    }
                    else
                    {
                        // Debug.LogWarning($"未找到 EventKey: {className}.{fieldName} (文件: {filePath})");
                    }
                }
            }
        }

        private int GetLineNumber(string content, int index, string[] lines)
        {
            int currentIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (index >= currentIndex && index < currentIndex + lines[i].Length + 1)
                {
                    return i;
                }
                currentIndex += lines[i].Length + 1;
            }
            return 0;
        }

        public static void MarkForRefresh()
        {
            var window = GetWindow<EventSubscriberWindow>();
            if (window != null)
            {
                window.RefreshEventKeyCache();
                window.RefreshAnalysis();
            }
        }
    
        private static string RemoveComments(string code)
        {
            //清除单行注释
            code = Regex.Replace(code, @"//.*", "");
            //清除多行注释
            code = Regex.Replace(code, @"/\*[\s\S]*?\*/", "");
            return code;
        } 
    }
}