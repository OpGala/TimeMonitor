using System;
using UnityEditor;
using UnityEngine;

namespace TimeMonitor.Editor
{
    public sealed class TimeMonitorEditor : EditorWindow
    {
        private const string AssetPath = "Assets/TimeMonitor/TimeMonitorData.asset";
        private static TimeMonitorData _timeMonitorData;

        private DateTime _sessionStart;
        private DateTime _sessionEnd;
        private double _currentSessionTime;
        private bool _sessionActive = false;

        [MenuItem("Tools/Time Monitor")]
        public static void ShowWindow()
        {
            GetWindow<TimeMonitorEditor>("Time Monitor");
            LoadTimeData();
        }

        private void OnGUI()
        {
            if (_timeMonitorData == null)
            {
                EditorGUILayout.LabelField("Time Monitor Data not found. Please create it via Assets/Create/TimeMonitor/TimeMonitorData.");
                if (GUILayout.Button("Create Time Monitor Data"))
                {
                    TimeMonitorDataCreator.CreateTimeMonitorData();
                    LoadTimeData();
                }
                return;
            }

            EditorGUILayout.LabelField("Total Time Spent:", FormatTime(_timeMonitorData.TotalTime));
            EditorGUILayout.LabelField("Current Session Time:", FormatTime(_currentSessionTime));
            EditorGUILayout.Space();

            if (GUILayout.Button("Update Time"))
            {
                UpdateTime();
            }

            if (_sessionActive)
            {
                if (GUILayout.Button("End Session"))
                {
                    EndSession();
                }
            }
            else
            {
                if (GUILayout.Button("Start Session"))
                {
                    StartSession();
                }
            }
        }

        private static void LoadTimeData()
        {
            _timeMonitorData = AssetDatabase.LoadAssetAtPath<TimeMonitorData>(AssetPath);
            if (_timeMonitorData == null)
            {
                Debug.LogWarning("TimeMonitorData asset not found at " + AssetPath);
            }
        }

        private void UpdateTime()
        {
            if (_sessionActive)
            {
                _currentSessionTime = (DateTime.Now - _sessionStart).TotalSeconds;
            }
        }

        private void StartSession()
        {
            _sessionStart = DateTime.Now;
            _sessionActive = true;
            _currentSessionTime = 0;
            Debug.Log("Session started at: " + _sessionStart);
        }

        private void EndSession()
        {
            _sessionEnd = DateTime.Now;
            _currentSessionTime = (_sessionEnd - _sessionStart).TotalSeconds;
            _timeMonitorData.TotalTime += _currentSessionTime;

            var sessionData = new TimeData
            {
                StartDate = _sessionStart.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = _sessionEnd.ToString("yyyy-MM-dd HH:mm:ss"),
                SessionTime = _currentSessionTime
            };
            _timeMonitorData.Sessions.Add(sessionData);

            _sessionActive = false;
            EditorUtility.SetDirty(_timeMonitorData); // Mark the data as dirty to ensure it gets saved
            Debug.Log("Session ended at: " + _sessionEnd + ", duration: " + _currentSessionTime);
        }

        private static string FormatTime(double time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
    }
    
    public static class TimeMonitorDataCreator
    {
        [MenuItem("Assets/Create/TimeMonitor/TimeMonitorData")]
        public static void CreateTimeMonitorData()
        {
            string assetPath = "Assets/TimeMonitor/TimeMonitorData.asset";
            if (AssetDatabase.LoadAssetAtPath<TimeMonitorData>(assetPath) != null)
            {
                Debug.LogWarning("TimeMonitorData asset already exists at " + assetPath);
                return;
            }

            TimeMonitorData asset = ScriptableObject.CreateInstance<TimeMonitorData>();
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log("TimeMonitorData asset created at " + assetPath);
        }
    }

}
