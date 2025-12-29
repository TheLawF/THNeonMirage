#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Fictology.UnityEditor
{
    public class EventCallViewer : EditorWindow
    {
        [MenuItem("Fictology/Event Center/事件调用栈分析器")]
        public static void ShowWindow()
        {
            var window = GetWindow<EventCallViewer>("事件调用栈分析器");
            window.minSize = new Vector2(1200, 700);
            window.Show();
        }

        [Serializable]
        public class EventCallInfo
        {
            public string eventKey;
            public string callerEventKey; // 调用者事件（如果是被事件触发的）
            public string callerMethod;   // 调用者方法
            public string filePath;
            public int lineNumber;
            public List<EventCallInfo> children = new List<EventCallInfo>(); // 被调用的事件
        }

        [Serializable]
        public class CycleDetectionResult
        {
            public List<string> cyclePath;
            public string cycleStartEvent;
            public bool hasCycle;
        }

        // 数据
        private Dictionary<string, EventCallInfo> eventCallGraph = new Dictionary<string, EventCallInfo>();
        private Dictionary<string, List<EventCallInfo>> eventTriggers = new Dictionary<string, List<EventCallInfo>>();
        private TreeViewState treeViewState;
        private EventCallTreeView treeView;
        private Vector2 scrollPosition;
        private string selectedEventKey;
        private CycleDetectionResult cycleResult;
        private bool showCyclesOnly = false;
        private string searchFilter = "";

        // 正则表达式
        private static readonly Regex methodDefinitionRegex = new Regex(
            @"(?:public|private|protected|internal)\s+(?:static\s+)?(?:void|[\w<>]+)\s+(\w+)\s*\([^)]*\)\s*\{",
            RegexOptions.Compiled);

        private static readonly Regex triggerEventRegex = new Regex(
            @"EventCenter\s*\.\s*TriggerEvent\s*(?:<\s*[^>]+\s*>)?\s*\(\s*EventRegistry\s*\.\s*(\w+)",
            RegexOptions.Compiled);

        private void OnEnable()
        {
            treeViewState = new TreeViewState();
            RefreshAnalysis();
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
                if (GUILayout.Button("分析调用栈", EditorStyles.toolbarButton, GUILayout.Width(100)))
                {
                    RefreshAnalysis();
                }

                if (GUILayout.Button("检测循环", EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    DetectCycles();
                }

                GUILayout.Space(10);

                showCyclesOnly = GUILayout.Toggle(showCyclesOnly, "仅显示循环", EditorStyles.toolbarButton, GUILayout.Width(100));

                GUILayout.FlexibleSpace();

                // 搜索框
                GUILayout.Label("搜索事件:", GUILayout.ExpandWidth(false));
                searchFilter = GUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(200));

                if (!string.IsNullOrEmpty(searchFilter) && GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    searchFilter = "";
                }
            }
            GUILayout.EndHorizontal();

            // 循环检测结果
            if (cycleResult != null && cycleResult.hasCycle)
            {
                EditorGUILayout.HelpBox($"检测到递归循环! 循环路径: {string.Join(" → ", cycleResult.cyclePath)}", MessageType.Error);
            }
            else if (cycleResult != null)
            {
                EditorGUILayout.HelpBox("未检测到递归循环", MessageType.Info);
            }
        }

        private void DrawContent()
        {
            if (eventCallGraph == null || eventCallGraph.Count == 0)
            {
                EditorGUILayout.HelpBox("点击\"分析调用栈\"按钮开始分析", MessageType.Info);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                // 左侧事件列表
                DrawEventList();
                
                // 右侧调用栈详情
                DrawCallStackDetails();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEventList()
        {
            GUILayout.BeginVertical(GUILayout.Width(300));
            {
                GUILayout.Label("事件列表", EditorStyles.boldLabel);
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
                {
                    var events = GetFilteredEvents();
                    
                    foreach (var eventKey in events)
                    {
                        DrawEventListItem(eventKey);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        private IEnumerable<string> GetFilteredEvents()
        {
            var events = eventCallGraph.Keys.OrderBy(k => k).ToList();
            
            if (!string.IsNullOrEmpty(searchFilter))
            {
                events = events.Where(e => e.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            
            if (showCyclesOnly && cycleResult != null && cycleResult.hasCycle)
            {
                // 只显示参与循环的事件
                events = events.Where(e => cycleResult.cyclePath.Contains(e)).ToList();
            }
            
            return events;
        }

        private void DrawEventListItem(string eventKey)
        {
            bool isSelected = selectedEventKey == eventKey;
            bool hasCycle = cycleResult != null && cycleResult.hasCycle && cycleResult.cyclePath.Contains(eventKey);
            
            GUIStyle style = isSelected ? EditorStyles.whiteLabel : EditorStyles.label;
            if (hasCycle)
            {
                style = new GUIStyle(style) { normal = { textColor = Color.red } };
            }
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(hasCycle ? "🔄 " + eventKey : eventKey, style, GUILayout.ExpandWidth(true)))
                {
                    selectedEventKey = eventKey;
                }
                
                // 显示调用次数
                int callCount = eventCallGraph[eventKey].children.Count;
                if (callCount > 0)
                {
                    GUILayout.Label($"({callCount})", EditorStyles.miniLabel, GUILayout.Width(30));
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawCallStackDetails()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            {
                if (string.IsNullOrEmpty(selectedEventKey) || !eventCallGraph.ContainsKey(selectedEventKey))
                {
                    EditorGUILayout.HelpBox("选择左侧的事件查看调用栈详情", MessageType.Info);
                }
                else
                {
                    var eventInfo = eventCallGraph[selectedEventKey];
                    
                    GUILayout.Label($"事件: {selectedEventKey}", EditorStyles.largeLabel);
                    
                    // 显示循环信息
                    if (cycleResult != null && cycleResult.hasCycle && cycleResult.cyclePath.Contains(selectedEventKey))
                    {
                        int index = cycleResult.cyclePath.IndexOf(selectedEventKey);
                        EditorGUILayout.HelpBox(
                            $"⚠️ 此事件参与递归循环!\n循环路径: {string.Join(" → ", cycleResult.cyclePath)}", 
                            MessageType.Error);
                    }
                    
                    // 显示被调用的事件
                    if (eventInfo.children.Count > 0)
                    {
                        GUILayout.Label("触发的事件:", EditorStyles.boldLabel);
                        foreach (var child in eventInfo.children)
                        {
                            DrawCallInfo(child, 1);
                        }
                    }
                    else
                    {
                        GUILayout.Label("此事件不触发其他事件", EditorStyles.helpBox);
                    }
                    
                    // 显示触发此事件的位置
                    if (eventTriggers.ContainsKey(selectedEventKey) && eventTriggers[selectedEventKey].Count > 0)
                    {
                        GUILayout.Label("被触发的位置:", EditorStyles.boldLabel);
                        foreach (var trigger in eventTriggers[selectedEventKey])
                        {
                            DrawTriggerInfo(trigger);
                        }
                    }
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawCallInfo(EventCallInfo callInfo, int indentLevel)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(indentLevel * 20);
                
                bool hasCycle = cycleResult != null && cycleResult.hasCycle && 
                               cycleResult.cyclePath.Contains(callInfo.eventKey);
                
                GUIStyle labelStyle = hasCycle ? 
                    new GUIStyle(EditorStyles.label) { normal = { textColor = Color.red } } : 
                    EditorStyles.label;
                
                // 事件名称
                if (GUILayout.Button(callInfo.eventKey, labelStyle, GUILayout.Width(200)))
                {
                    selectedEventKey = callInfo.eventKey;
                }
                
                // 调用位置
                string fileName = Path.GetFileName(callInfo.filePath);
                if (GUILayout.Button($"{fileName}:{callInfo.lineNumber}", EditorStyles.miniButton, GUILayout.Width(120)))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(callInfo.filePath, callInfo.lineNumber);
                }
                
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            
            // 递归显示子调用
            foreach (var child in callInfo.children)
            {
                DrawCallInfo(child, indentLevel + 1);
            }
        }

        private void DrawTriggerInfo(EventCallInfo triggerInfo)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                
                string triggerText = string.IsNullOrEmpty(triggerInfo.callerEventKey) ? 
                    "直接调用" : $"被事件 {triggerInfo.callerEventKey} 触发";
                
                GUILayout.Label(triggerText, GUILayout.Width(200));
                
                string fileName = Path.GetFileName(triggerInfo.filePath);
                if (GUILayout.Button($"{fileName}:{triggerInfo.lineNumber}", EditorStyles.miniButton, GUILayout.Width(120)))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(triggerInfo.filePath, triggerInfo.lineNumber);
                }
                
                if (!string.IsNullOrEmpty(triggerInfo.callerMethod))
                {
                    GUILayout.Label($"方法: {triggerInfo.callerMethod}", EditorStyles.miniLabel);
                }
                
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void RefreshAnalysis()
        {
            eventCallGraph.Clear();
            eventTriggers.Clear();
            cycleResult = null;
            
            // 获取所有C#脚本
            var scriptFiles = AssetDatabase.FindAssets("t:Script")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.StartsWith("Assets/") && path.EndsWith(".cs"))
                .ToArray();

            int processedFiles = 0;
            foreach (var filePath in scriptFiles)
            {
                processedFiles++;
                string fullPath = Application.dataPath + "/../" + filePath;
                
                EditorUtility.DisplayProgressBar("分析事件调用栈", 
                    $"正在分析 {Path.GetFileName(filePath)}", 
                    (float)processedFiles / scriptFiles.Length);

                try
                {
                    AnalyzeFile(fullPath, filePath);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"分析文件 {filePath} 时出错: {ex.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            Repaint();
            
            Debug.Log($"分析完成: 发现 {eventCallGraph.Count} 个事件的调用关系");
        }

        private void AnalyzeFile(string fullPath, string assetPath)
        {
            if (!File.Exists(fullPath)) return;
            
            string content = File.ReadAllText(fullPath);
            string[] lines = File.ReadAllLines(fullPath);
            
            // 分析方法定义和事件触发
            AnalyzeMethodCalls(content, lines, assetPath);
        }

        private void AnalyzeMethodCalls(string content, string[] lines, string filePath)
        {
            // 查找所有方法定义
            var methodMatches = methodDefinitionRegex.Matches(content);
            var methods = new Dictionary<string, int>(); // 方法名 -> 开始位置
            
            foreach (Match match in methodMatches)
            {
                if (match.Groups.Count > 1)
                {
                    string methodName = match.Groups[1].Value;
                    methods[methodName] = match.Index;
                }
            }
            
            // 查找所有事件触发
            var triggerMatches = triggerEventRegex.Matches(content);
            foreach (Match match in triggerMatches)
            {
                if (match.Groups.Count > 1)
                {
                    string eventKey = match.Groups[1].Value;
                    int lineNumber = GetLineNumber(content, match.Index, lines) + 1;
                    
                    // 确定调用者方法
                    string callerMethod = FindCallerMethod(methods, match.Index);
                    
                    // 记录事件触发
                    RecordEventTrigger(eventKey, filePath, lineNumber, callerMethod);
                }
            }
            
            // 构建调用图（简化版：基于方法调用顺序）
            BuildCallGraphBasedOnMethodOrder(content, filePath, lines);
        }

        private string FindCallerMethod(Dictionary<string, int> methods, int triggerPosition)
        {
            // 找到触发位置之前最近的方法定义
            var precedingMethods = methods.Where(m => m.Value < triggerPosition)
                                         .OrderByDescending(m => m.Value)
                                         .ToList();
            
            return precedingMethods.Count > 0 ? precedingMethods[0].Key : "未知方法";
        }

        private void RecordEventTrigger(string eventKey, string filePath, int lineNumber, string callerMethod)
        {
            var callInfo = new EventCallInfo
            {
                eventKey = eventKey,
                callerMethod = callerMethod,
                filePath = filePath,
                lineNumber = lineNumber
            };
            
            // 添加到触发记录
            if (!eventTriggers.ContainsKey(eventKey))
            {
                eventTriggers[eventKey] = new List<EventCallInfo>();
            }
            eventTriggers[eventKey].Add(callInfo);
            
            // 确保事件在调用图中存在
            if (!eventCallGraph.ContainsKey(eventKey))
            {
                eventCallGraph[eventKey] = new EventCallInfo { eventKey = eventKey };
            }
        }

        private void BuildCallGraphBasedOnMethodOrder(string content, string filePath, string[] lines)
        {
            // 这是一个简化的调用图构建，基于方法中事件触发的顺序
            // 实际项目中可能需要更复杂的静态分析
            
            var methodCalls = new Dictionary<string, List<string>>(); // 方法 -> 触发的事件列表
            
            // 分析每个方法中触发的事件顺序
            var methodMatches = methodDefinitionRegex.Matches(content);
            foreach (Match methodMatch in methodMatches)
            {
                if (methodMatch.Groups.Count < 2) continue;
                
                string methodName = methodMatch.Groups[1].Value;
                int methodStart = methodMatch.Index + methodMatch.Length;
                
                // 查找方法结束（简化版：查找匹配的括号）
                int methodEnd = FindMethodEnd(content, methodStart);
                if (methodEnd < 0) continue;
                
                string methodBody = content.Substring(methodStart, methodEnd - methodStart);
                var triggersInMethod = triggerEventRegex.Matches(methodBody)
                    .Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .ToList();
                
                if (triggersInMethod.Count > 0)
                {
                    methodCalls[methodName] = triggersInMethod;
                }
            }
            
            // 构建调用链：如果方法A触发事件B，而方法B（同名）触发事件C，则建立A->B->C的调用链
            foreach (var methodPair in methodCalls)
            {
                string methodName = methodPair.Key;
                var triggeredEvents = methodPair.Value;
                
                // 查找调用此方法的事件
                var callingEvents = eventTriggers
                    .Where(pair => pair.Value.Any(call => call.callerMethod == methodName))
                    .Select(pair => pair.Key)
                    .ToList();
                
                foreach (string callingEvent in callingEvents)
                {
                    foreach (string triggeredEvent in triggeredEvents)
                    {
                        AddCallRelationship(callingEvent, triggeredEvent, filePath);
                    }
                }
            }
        }

        private int FindMethodEnd(string content, int startIndex)
        {
            int braceCount = 0;
            bool inString = false;
            char stringChar = '\0';
            
            for (int i = startIndex; i < content.Length; i++)
            {
                char c = content[i];
                
                // 处理字符串字面量
                if (!inString && (c == '"' || c == '\''))
                {
                    inString = true;
                    stringChar = c;
                }
                else if (inString && c == stringChar && content[i-1] != '\\')
                {
                    inString = false;
                }
                
                if (inString) continue;
                
                // 计数大括号
                if (c == '{') braceCount++;
                else if (c == '}') braceCount--;
                
                // 找到匹配的结束括号
                if (braceCount == 0) return i + 1;
            }
            
            return -1;
        }

        private void AddCallRelationship(string fromEvent, string toEvent, string filePath)
        {
            // 确保源事件存在
            if (!eventCallGraph.ContainsKey(fromEvent))
            {
                eventCallGraph[fromEvent] = new EventCallInfo { eventKey = fromEvent };
            }
            
            // 确保目标事件存在
            if (!eventCallGraph.ContainsKey(toEvent))
            {
                eventCallGraph[toEvent] = new EventCallInfo { eventKey = toEvent };
            }
            
            // 添加调用关系
            var callInfo = new EventCallInfo
            {
                eventKey = toEvent,
                callerEventKey = fromEvent,
                filePath = filePath,
                lineNumber = 0 // 简化版，实际应该记录具体行号
            };
            
            eventCallGraph[fromEvent].children.Add(callInfo);
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

        private void DetectCycles()
        {
            cycleResult = new CycleDetectionResult { hasCycle = false, cyclePath = new List<string>() };
            
            var visited = new Dictionary<string, bool>(); // false: 访问中, true: 已访问完成
            var recursionStack = new Stack<string>();
            var path = new List<string>();
            
            foreach (string eventKey in eventCallGraph.Keys)
            {
                if (DetectCycleDFS(eventKey, visited, recursionStack, path))
                {
                    cycleResult.hasCycle = true;
                    cycleResult.cyclePath = new List<string>(path);
                    cycleResult.cycleStartEvent = path.First();
                    break;
                }
            }
            
            Repaint();
            
            if (cycleResult.hasCycle)
            {
                Debug.LogError($"检测到递归循环: {string.Join(" → ", cycleResult.cyclePath)}");
            }
            else
            {
                Debug.Log("未检测到递归循环");
            }
        }

        private bool DetectCycleDFS(string currentEvent, Dictionary<string, bool> visited, Stack<string> recursionStack, List<string> currentPath)
        {
            if (!eventCallGraph.ContainsKey(currentEvent)) return false;
            
            // 如果当前节点正在递归栈中，说明检测到循环
            if (recursionStack.Contains(currentEvent))
            {
                // 构建循环路径
                currentPath.Clear();
                bool startRecording = false;
                
                foreach (string eventKey in recursionStack)
                {
                    if (eventKey == currentEvent) startRecording = true;
                    if (startRecording) currentPath.Add(eventKey);
                }
                currentPath.Add(currentEvent); // 闭合循环
                return true;
            }
            
            // 如果已经访问完成，直接返回
            if (visited.ContainsKey(currentEvent) && visited[currentEvent])
                return false;
            
            // 标记为访问中
            visited[currentEvent] = false;
            recursionStack.Push(currentEvent);
            
            // 递归检查所有子节点
            foreach (var child in eventCallGraph[currentEvent].children)
            {
                if (DetectCycleDFS(child.eventKey, visited, recursionStack, currentPath))
                    return true;
            }
            
            // 标记为访问完成
            recursionStack.Pop();
            visited[currentEvent] = true;
            
            return false;
        }
    }

    // 简单的树视图实现（如果需要更复杂的树形显示可以使用这个）
    public class EventCallTreeView : TreeView
    {
        public EventCallTreeView(TreeViewState state) : base(state)
        {
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            // 这里可以构建树形结构
            return root;
        }
    }
}
#endif