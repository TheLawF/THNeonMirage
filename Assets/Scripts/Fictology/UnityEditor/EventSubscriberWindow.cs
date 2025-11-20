using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FlyRabbit.EventCenter.Core;
using UnityEditor;
using UnityEngine;

namespace Fictology.UnityEditor
{
    public class EventSubscriberWindow: EditorWindow
    {
        [MenuItem("Fictology/EventCenter/事件调用查看器")]
        public static void CreateWindow()
        {
            GetWindow<EventSubscriberWindow>("事件调用查看器");
        }
        
        private Dictionary<EventName, List<EventReference>> eventReferences = new();
        private Vector2 scrollPosition;
        private EventName? selectedEvent;
        private bool showListeners = true;
        private bool showTriggers = true;
        private string searchFilter = "";
        
        /// <summary>
        /// 提示文本-中文
        /// </summary>
        private static List<string> m_ScriptPaths = new ();
        private static string m_NotesText = "事件查看器使用正则表达式搜索项目中的以下方法的调用来工作：\nEventCenter.AddListener\nEventCenter.RemoveListener\nEventCenter.TriggerEvent\n如果你的项目中有其他的\"EventCenter\"类，并且也拥有这些方法，那么事件查看器可能无法正常工作。\n此外，事件查看器会忽略Editor文件夹。";
        
        private static readonly Regex m_AddRegex = new Regex(@"(?<!""[^\s]*)EventCenter\s*\.\s*AddListener\s*(?:<\s*(?<Types>[^>]+)\s*>)?\s*\(\s*EventName\s*\.\s*(?<Name>\w+)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex m_RemoveRegex = new Regex(@"(?<!""[^\s]*)EventCenter\s*\.\s*RemoveListener\s*(?:<\s*(?<Types>[^>]+)\s*>)?\s*\(\s*EventName\s*\.\s*(?<Name>\w+)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex m_TriggerRegex = new Regex(@"(?<!""[^\s]*)EventCenter\s*\.\s*TriggerEvent\s*(?:<\s*(?<Types>[^>]+)\s*>)?\s*\(\s*EventName\s*\.\s*(?<Name>\w+)", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// key为事件名，value为对应的group
        /// </summary>
        private static readonly Dictionary<string, EventReference> m_Events = new();
        /// <summary>
        /// key为事件名，value为此foldout是否折叠
        /// </summary>
        private static readonly Dictionary<string, bool> m_foldouts = new();
        private void OnEnable()
        {
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
                if (GUILayout.Button("刷新分析", EditorStyles.toolbarButton))
                {
                    RefreshAnalysis();
                }
                
                showListeners = GUILayout.Toggle(showListeners, "显示监听", EditorStyles.toolbarButton);
                showTriggers = GUILayout.Toggle(showTriggers, "显示触发", EditorStyles.toolbarButton);
                
                GUILayout.FlexibleSpace();
                GUILayout.Label("搜索:", GUILayout.ExpandWidth(false));
                searchFilter = GUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(200));
            }
            GUILayout.EndHorizontal();
        }

        private void DrawContent()
        {
            if (eventReferences == null)
            {
                EditorGUILayout.HelpBox("点击刷新分析按钮开始分析事件中心使用情况", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                var filteredEvents = GetFilteredEvents();
                foreach (var eventName in filteredEvents)
                {
                    DrawEventSection(eventName);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private IEnumerable<EventName> GetFilteredEvents()
        {
            var events = eventReferences.Keys.OrderBy(x => x.ToString());
            
            if (!string.IsNullOrEmpty(searchFilter))
            {
                events = events.Where(x => x.ToString().Contains(searchFilter, StringComparison.OrdinalIgnoreCase))
                              .OrderBy(x => x.ToString());
            }
            
            return events;
        }

        private void DrawEventSection(EventName eventName)
        {
            var references = eventReferences[eventName];
            var listenerCount = references.Count(r => r.type == EventReferenceType.AddListener || r.type == EventReferenceType.RemoveListener);
            var triggerCount = references.Count(r => r.type == EventReferenceType.TriggerEvent);

            // 事件标题
            bool isExpanded = selectedEvent == eventName;
            bool newExpanded = EditorGUILayout.Foldout(isExpanded, 
                $"{eventName} (监听: {listenerCount}, 触发: {triggerCount})", true);
            
            if (newExpanded != isExpanded)
            {
                selectedEvent = newExpanded ? eventName : null;
            }

            if (newExpanded)
            {
                EditorGUI.indentLevel++;
                DrawEventReferences(references);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
        }

        private void DrawEventReferences(List<EventReference> references)
        {
            // 监听器部分
            if (showListeners)
            {
                var listeners = references.Where(r => r.type == EventReferenceType.AddListener).ToList();
                if (listeners.Count > 0)
                {
                    EditorGUILayout.LabelField("监听器:", EditorStyles.boldLabel);
                    foreach (var reference in listeners)
                    {
                        DrawReferenceItem(reference);
                    }
                }

                var removers = references.Where(r => r.type == EventReferenceType.RemoveListener).ToList();
                if (removers.Count > 0)
                {
                    EditorGUILayout.LabelField("移除监听:", EditorStyles.boldLabel);
                    foreach (var reference in removers)
                    {
                        DrawReferenceItem(reference);
                    }
                }
            }

            // 触发器部分
            if (showTriggers)
            {
                var triggers = references.Where(r => r.type == EventReferenceType.TriggerEvent).ToList();
                if (triggers.Count > 0)
                {
                    EditorGUILayout.LabelField("触发位置:", EditorStyles.boldLabel);
                    foreach (var reference in triggers)
                    {
                        DrawReferenceItem(reference);
                    }
                }
            }
        }

        private void DrawReferenceItem(EventReference reference)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // 显示类型图标和文本
                string typeText = reference.type switch
                {
                    EventReferenceType.AddListener => "📞 添加监听",
                    EventReferenceType.RemoveListener => "❌ 移除监听", 
                    EventReferenceType.TriggerEvent => "🎯 触发事件",
                    _ => "未知"
                };

                EditorGUILayout.LabelField(typeText, GUILayout.Width(100));
                
                // 显示泛型信息
                if (!string.IsNullOrEmpty(reference.genericTypes))
                {
                    EditorGUILayout.LabelField($"<{reference.genericTypes}>", GUILayout.Width(120));
                }
                else
                {
                    GUILayout.Space(120);
                }

                // 显示文件信息
                var fileName = Path.GetFileName(reference.filePath);
                EditorGUILayout.LabelField($"{fileName}:{reference.lineNumber}", GUILayout.Width(150));

                GUILayout.FlexibleSpace();

                // 跳转按钮
                if (GUILayout.Button("跳转", GUILayout.Width(60)))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(reference.filePath, reference.lineNumber);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RefreshAnalysis()
        {
            m_ScriptPaths.Clear();
            m_foldouts.Clear();
            m_Events.Clear();
            eventReferences.Clear();

            //获取所有Assets目录下的脚本的GUID
            var scriptGuids = AssetDatabase.FindAssets("t:Script", new [] { "Assets" });
            
            //GUID转为路径，并排除Editor文件夹下的脚本，存储至m_ScriptPaths
            foreach (var item in scriptGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                if (path.Contains("/Editor/"))
                {
                    continue;
                }
                m_ScriptPaths.Add(path);
            }
            //遍历每一个文件
            foreach (string scriptPath in m_ScriptPaths)
            {
                //获得不带注释的源码
                string code;
                string fullPath = Path.GetFullPath(scriptPath);
                code = File.ReadAllText(fullPath);
                code = RemoveComments(code);
                //进行匹配
                AnalyzeFile(fullPath);
            }
            //生成事件对应的foldout需要的参数
            foreach (var item in m_Events)
            {
                m_foldouts[item.Key] = false;
            }

            EditorUtility.ClearProgressBar();
            Repaint();
        }

        private void AnalyzeFile(string filePath)
        {
            var content = File.ReadAllText(filePath);
            content = RemoveComments(content);
            var lines = content.Split("\n");

            AnalyzeWithRegex(content, lines, filePath, m_AddRegex, EventReferenceType.AddListener);
            AnalyzeWithRegex(content, lines, filePath, m_RemoveRegex, EventReferenceType.RemoveListener);
            AnalyzeWithRegex(content, lines, filePath, m_TriggerRegex, EventReferenceType.TriggerEvent);
        }

        private void AnalyzeWithRegex(string content, string[] lines, string filePath, Regex regex, EventReferenceType referenceType)
        {
            var matches = regex.Matches(content);
            foreach (Match match in matches)
            {
                if (!match.Groups["Name"].Success) continue;
                var eventNameStr = match.Groups["Name"].Value;
                
                if (!Enum.TryParse<EventName>(eventNameStr, out EventName eventName)) continue;
                var genericTypes = match.Groups["Types"].Success ? match.Groups["Types"].Value : "";
                        
                // 计算行号
                var lineNumber = GetLineNumber(content, match.Index, lines) + 1;
                var reference = new EventReference
                {
                    type = referenceType,
                    eventName = eventName,
                    filePath = filePath,
                    lineNumber = lineNumber,
                    genericTypes = genericTypes
                };

                Debug.Log(string.Join(';', eventReferences));
                if (!eventReferences.ContainsKey(eventName))
                {
                    eventReferences[eventName] = new List<EventReference>();
                }

                eventReferences[eventName].Add(reference);
            }
        }

        private int GetLineNumber(string content, int index, string[] lines)
        {
            int currentIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (index >= currentIndex && index < currentIndex + lines[i].Length + 1) // +1 for newline
                {
                    return i;
                }
                currentIndex += lines[i].Length + 1;
            }
            return 0;
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