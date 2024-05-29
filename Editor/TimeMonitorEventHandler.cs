using UnityEditor;

namespace TimeMonitor.Editor
{
    [InitializeOnLoad]
    public static class TimeMonitorEventHandler
    {
        static TimeMonitorEventHandler()
        {
            EditorApplication.quitting += OnEditorQuitting;
        }

        private static void OnEditorQuitting()
        {
            if (EditorWindow.HasOpenInstances<TimeMonitorWindow>())
            {
                var window = EditorWindow.GetWindow<TimeMonitorWindow>();
                window.EndCurrentSession();
            }
        }
    }
}

