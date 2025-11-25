#if UNITY_EDITOR
using Fictology.Event;
using UnityEditor;

namespace Fictology.UnityEditor
{
    [InitializeOnLoad]
    public static class EventEditorIntegration
    {
        static EventEditorIntegration()
        {
            // 注册编译完成事件
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        
            // 注册编辑器更新事件（用于延迟初始化）
            EditorApplication.update += InitializeOnFirstUpdate;
        }

        private static bool _isInitialized = false;
    
        private static void InitializeOnFirstUpdate()
        {
            if (_isInitialized) 
            {
                EditorApplication.update -= InitializeOnFirstUpdate;
                return;
            }
        
            // 延迟初始化，确保所有程序集已加载
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;
            
            RefreshEventKeyCache();
            _isInitialized = true;
            EditorApplication.update -= InitializeOnFirstUpdate;
        }

        private static void OnAfterAssemblyReload()
        {
            // 编译完成后刷新缓存
            RefreshEventKeyCache();
        }

        public static void RefreshEventKeyCache()
        {
            EventHelper.RefreshCache();
        }
    }
}
#endif