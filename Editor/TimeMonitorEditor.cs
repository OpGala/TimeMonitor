using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TimeMonitor.Editor
{
    public sealed class TimeMonitorEditor : EditorWindow
    {
        private double _totalTime;
        private double _sessionTime;
        private readonly List<double> _sessionTimes = new();
        private string _filePath;
        private double _startTime;
        private static TimeMonitorEditor _window;

        [MenuItem("Tools/Time Monitor")]
        public static void ShowWindow()
        {
            _window = GetWindow<TimeMonitorEditor>("Time Monitor");
            _window.LoadTime();
            _window._startTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += _window.Update;
            EditorApplication.quitting += _window.OnEditorQuit;
        }

        private void OnDestroy()
        {
            EditorApplication.update -= Update;
            EditorApplication.quitting -= OnEditorQuit;
            SaveTime();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Total Time Spent in Editor:", FormatTime(_totalTime));
            EditorGUILayout.LabelField("Current Session Time:", FormatTime(_sessionTime));
            EditorGUILayout.Space();

            if (GUILayout.Button("Clear Time Data"))
            {
                ClearTimeData();
            }
        }

        private void Update()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPaused)
            {
                return;
            }

            double currentTime = EditorApplication.timeSinceStartup;
            double deltaTime = currentTime - _startTime;
            _totalTime += deltaTime;
            _sessionTime += deltaTime;
            _startTime = currentTime;

            Repaint();
        }

        private void OnEditorQuit()
        {
            _sessionTimes.Add(_sessionTime);
            SaveTime();
        }

        private void LoadTime()
        {
            _filePath = Path.Combine(Application.persistentDataPath, "TimeMonitorLogs.txt");
            Debug.Log($"Loading time from {_filePath}");

            try
            {
                if (File.Exists(_filePath))
                {
                    string[] data = File.ReadAllLines(_filePath);
                    if (data.Length > 0 && double.TryParse(data[0], out double savedTime))
                    {
                        _totalTime = savedTime;
                    }

                    if (data.Length > 1)
                    {
                        for (int i = 1; i < data.Length; i++)
                        {
                            if (double.TryParse(data[i], out double session))
                            {
                                _sessionTimes.Add(session);
                            }
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.LogError("Error reading time log file: " + ex.Message);
            }
        }

        private void SaveTime()
        {
            var data = new List<string> { _totalTime.ToString(CultureInfo.CurrentCulture) };
            data.AddRange(_sessionTimes.ConvertAll(s => s.ToString(CultureInfo.CurrentCulture)));
            Debug.Log($"Saving time to {_filePath}");

            try
            {
                File.WriteAllLines(_filePath, data);
                Debug.Log("Time saved successfully");
            }
            catch (IOException ex)
            {
                Debug.LogError("Error writing time log file: " + ex.Message);
            }
        }

        private void ClearTimeData()
        {
            _totalTime = 0;
            _sessionTime = 0;
            _sessionTimes.Clear();
            try
            {
                File.Delete(_filePath);
                Debug.Log("Time data cleared");
            }
            catch (IOException ex)
            {
                Debug.LogError("Error deleting time log file: " + ex.Message);
            }
            Repaint();
        }

        private static string FormatTime(double time)
        {
            int hours = Mathf.FloorToInt((float)time / 3600);
            int minutes = Mathf.FloorToInt(((float)time % 3600) / 60);
            int seconds = Mathf.FloorToInt((float)time % 60);
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }
    }
}