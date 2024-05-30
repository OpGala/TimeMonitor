using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TimeMonitor.Data;
using UnityEditor;
using UnityEngine;

namespace TimeMonitor.Editor
{
    public sealed class TimeMonitorWindow : EditorWindow
    {
        private static TimeMonitorData _timeMonitorData;
        private Session _currentSession;
        private const string DataPath = "Assets/TimeMonitor/Data/TimeMonitorData.asset";
        private DateTime _sessionStartTime;
        private Vector2 _scrollPosition;
        private int _selectedYearIndex;
        private int _selectedMonthIndex;
        private string[] _years;
        private readonly string[] _months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private bool _showCalendar;
        private bool _showLineGraph;
        private bool _showHistogram;
        private bool _showReports;
        private bool _showAnalysis;

        private string _startDateString = DateTime.Now.ToString("yyyy-MM-dd");
        private string _endDateString = DateTime.Now.ToString("yyyy-MM-dd");
        private static string _selectedCodeEditor = "None";

        [MenuItem("Tools/Time Monitor")]
        public static void ShowWindow()
        {
            GetWindow<TimeMonitorWindow>("Time Monitor");
        }

        private void OnEnable()
        {
            LoadOrCreateData();
            StartNewSession();
        }

        private void OnDisable()
        {
            EndCurrentSession();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Settings", EditorStyles.toolbarButton))
            {
                SettingsWindow.ShowSettingWindow();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Current Session Time:", GetCurrentSessionTime());
            EditorGUILayout.LabelField("Total Project Time:", GetTotalProjectTime());
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(_showCalendar ? "Hide Calendar" : "Show Calendar"))
            {
                _showCalendar = !_showCalendar;
                _showLineGraph = false;
                _showHistogram = false;
                _showReports = false;
                _showAnalysis = false;
            }
            if (GUILayout.Button(_showLineGraph ? "Hide Line Graph" : "Show Line Graph"))
            {
                _showLineGraph = !_showLineGraph;
                _showCalendar = false;
                _showHistogram = false;
                _showReports = false;
                _showAnalysis = false;
            }
            if (GUILayout.Button(_showHistogram ? "Hide Histogram" : "Show Histogram"))
            {
                _showHistogram = !_showHistogram;
                _showCalendar = false;
                _showLineGraph = false;
                _showReports = false;
                _showAnalysis = false;
            }
            if (GUILayout.Button(_showReports ? "Hide Reports" : "Show Reports"))
            {
                _showReports = !_showReports;
                _showCalendar = false;
                _showLineGraph = false;
                _showHistogram = false;
                _showAnalysis = false;
            }
            if (GUILayout.Button(_showAnalysis ? "Hide Analysis" : "Show Analysis"))
            {
                _showAnalysis = !_showAnalysis;
                _showCalendar = false;
                _showLineGraph = false;
                _showHistogram = false;
                _showReports = false;
            }
            EditorGUILayout.EndHorizontal();

            _startDateString = EditorGUILayout.TextField("Start Date (yyyy-MM-dd)", _startDateString);
            _endDateString = EditorGUILayout.TextField("End Date (yyyy-MM-dd)", _endDateString);

            if (_showCalendar)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(400));
                DisplaySessionHistory();
                EditorGUILayout.EndScrollView();
            }
            else if (_showLineGraph)
            {
                DisplayLineGraph();
            }
            else if (_showHistogram)
            {
                DisplayHistogram();
            }
            else if (_showReports)
            {
                DisplayReports();
            }
            else if (_showAnalysis)
            {
                DisplayAnalysis();
            }
        }

        private static void LoadOrCreateData()
        {
            _timeMonitorData = AssetDatabase.LoadAssetAtPath<TimeMonitorData>(DataPath);
            if (_timeMonitorData == null)
            {
                _timeMonitorData = CreateInstance<TimeMonitorData>();
                if (!Directory.Exists("Assets/TimeMonitor/Data"))
                {
                    Directory.CreateDirectory("Assets/TimeMonitor/Data");
                }
                AssetDatabase.CreateAsset(_timeMonitorData, DataPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void StartNewSession()
        {
            _sessionStartTime = DateTime.Now;
            _currentSession = new Session
            {
                startTime = _sessionStartTime.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public void EndCurrentSession()
        {
            if (_currentSession != null)
            {
                _currentSession.endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                int year = _sessionStartTime.Year;
                int month = _sessionStartTime.Month;
                int day = _sessionStartTime.Day;

                EnsureDataStructure(year, month, day);
                _timeMonitorData.years[year - _timeMonitorData.startYear].months[month - 1].days[day - 1].sessions.Add(_currentSession);
                EditorUtility.SetDirty(_timeMonitorData);
                AssetDatabase.SaveAssets();
            }
        }

        private static void EnsureDataStructure(int year, int month, int day)
        {
            int startYear = _timeMonitorData.startYear;
            if (_timeMonitorData.years.Count == 0)
            {
                startYear = year;
                _timeMonitorData.startYear = startYear;
            }

            while (_timeMonitorData.years.Count <= (year - startYear))
            {
                var newYear = new Year { yearNumber = startYear + _timeMonitorData.years.Count };
                _timeMonitorData.years.Add(newYear);
            }
            Year currentYear = _timeMonitorData.years[year - startYear];

            while (currentYear.months.Count < month)
            {
                var newMonth = new Month { monthNumber = currentYear.months.Count + 1 };
                currentYear.months.Add(newMonth);
            }
            Month currentMonth = currentYear.months[month - 1];

            while (currentMonth.days.Count < day)
            {
                var newDay = new Day { dayNumber = currentMonth.days.Count + 1 };
                currentMonth.days.Add(newDay);
            }
        }

        private string GetCurrentSessionTime()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan elapsed = currentTime - _sessionStartTime;
            return elapsed.ToString(@"hh\:mm\:ss");
        }

        private static string GetTotalProjectTime()
        {
            var total = new TimeSpan();
            foreach (Year year in _timeMonitorData.years)
            {
                foreach (Month month in year.months)
                {
                    foreach (Day day in month.days)
                    {
                        foreach (Session session in day.sessions)
                        {
                            DateTime start = DateTime.Parse(session.startTime);
                            DateTime end = DateTime.Parse(session.endTime);
                            total += end - start;
                        }
                    }
                }
            }
            return total.ToString(@"hh\:mm\:ss");
        }

        private void DisplaySessionHistory()
        {
            if (_timeMonitorData.years.Count == 0)
            {
                EditorGUILayout.LabelField("No session data available.");
                return;
            }

            // Update years array
            _years = new string[_timeMonitorData.years.Count];
            for (int i = 0; i < _timeMonitorData.years.Count; i++)
            {
                _years[i] = _timeMonitorData.years[i].yearNumber.ToString();
            }

            _selectedYearIndex = GUILayout.Toolbar(_selectedYearIndex, _years);
            Year selectedYear = _timeMonitorData.years[_selectedYearIndex];

            _selectedMonthIndex = GUILayout.Toolbar(_selectedMonthIndex, _months);
            Month selectedMonth = selectedYear.months.Count > _selectedMonthIndex ? selectedYear.months[_selectedMonthIndex] : null;

            if (selectedMonth != null)
            {
                DisplayMonthCalendar(selectedMonth);
            }
            else
            {
                EditorGUILayout.LabelField("No data for the selected month.");
            }
        }

        private void DisplayMonthCalendar(Month selectedMonth)
        {
            int daysInMonth = DateTime.DaysInMonth(_timeMonitorData.startYear + _selectedYearIndex, selectedMonth.monthNumber);
            int firstDayOfWeek = (int)new DateTime(_timeMonitorData.startYear + _selectedYearIndex, selectedMonth.monthNumber, 1).DayOfWeek;

            GUILayout.BeginHorizontal();
            for (int i = 0; i < 7; i++)
            {
                GUILayout.Label(Enum.GetName(typeof(DayOfWeek), i), GUILayout.Width((Screen.width - 40) / 7));
            }
            GUILayout.EndHorizontal();

            int dayCounter = 1 - firstDayOfWeek;

            for (int i = 0; i < 6; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < 7; j++)
                {
                    if (dayCounter < 1 || dayCounter > daysInMonth)
                    {
                        GUILayout.Label("", GUILayout.Width((Screen.width - 40) / 7), GUILayout.Height(60));
                    }
                    else
                    {
                        Day day = selectedMonth.days.Find(d => d.dayNumber == dayCounter);
                        float totalDayTime = 0;
                        if (day != null)
                        {
                            foreach (Session session in day.sessions)
                            {
                                DateTime start = DateTime.Parse(session.startTime);
                                DateTime end = DateTime.Parse(session.endTime);
                                totalDayTime += (float)(end - start).TotalHours;
                            }
                        }

                        string dayLabel = $"{dayCounter}\n{totalDayTime:F2}h";
                        GUILayout.Button(dayLabel, GUILayout.Width((Screen.width - 40) / 7), GUILayout.Height(60));
                    }
                    dayCounter++;
                }
                GUILayout.EndHorizontal();
            }
        }

        private void DisplayLineGraph()
        {
            if (!DateTime.TryParse(_startDateString, out DateTime startDate) || !DateTime.TryParse(_endDateString, out DateTime endDate))
            {
                EditorGUILayout.LabelField("Invalid date format. Please use yyyy-MM-dd.");
                return;
            }

            var data = GetSessionDataInRange(startDate, endDate);
            if (data == null)
            {
                EditorGUILayout.LabelField("No session data available for the selected range.");
                return;
            }

            float[] dailyHours = data.Item1;
            int daysInRange = data.Item2;

            GUILayout.Label("Line Graph of Hours Worked per Day", EditorStyles.boldLabel);

            Rect graphRect = GUILayoutUtility.GetRect(600, 300);
            if (Event.current.type == EventType.Repaint)
            {
                Handles.color = Color.black;
                Handles.DrawLine(new Vector3(graphRect.x, graphRect.y), new Vector3(graphRect.x, graphRect.y + graphRect.height));
                Handles.DrawLine(new Vector3(graphRect.x, graphRect.y + graphRect.height), new Vector3(graphRect.x + graphRect.width, graphRect.y + graphRect.height));

                float maxHours = dailyHours.Max();
                float xStep = graphRect.width / daysInRange;
                float yStep = graphRect.height / maxHours;

                Handles.color = Color.blue;
                for (int i = 0; i < daysInRange - 1; i++)
                {
                    var start = new Vector3(graphRect.x + i * xStep, graphRect.y + graphRect.height - dailyHours[i] * yStep);
                    var end = new Vector3(graphRect.x + (i + 1) * xStep, graphRect.y + graphRect.height - dailyHours[i + 1] * yStep);
                    Handles.DrawLine(start, end);

                    Handles.DrawSolidDisc(start, Vector3.forward, 2);
                }

                Handles.color = Color.red;
                for (int i = 0; i < daysInRange; i++)
                {
                    var point = new Vector3(graphRect.x + i * xStep, graphRect.y + graphRect.height - dailyHours[i] * yStep);
                    Handles.DrawSolidDisc(point, Vector3.forward, 2);
                }
            }
        }

        private void DisplayHistogram()
        {
            if (!DateTime.TryParse(_startDateString, out DateTime startDate) || !DateTime.TryParse(_endDateString, out DateTime endDate))
            {
                EditorGUILayout.LabelField("Invalid date format. Please use yyyy-MM-dd.");
                return;
            }

            var data = GetSessionDataInRange(startDate, endDate);
            if (data == null)
            {
                EditorGUILayout.LabelField("No session data available for the selected range.");
                return;
            }

            float[] dailyHours = data.Item1;
            int daysInRange = data.Item2;

            GUILayout.Label("Histogram of Hours Worked per Day", EditorStyles.boldLabel);

            Rect histogramRect = GUILayoutUtility.GetRect(600, 300);
            if (Event.current.type == EventType.Repaint)
            {
                Handles.color = Color.black;
                Handles.DrawLine(new Vector3(histogramRect.x, histogramRect.y), new Vector3(histogramRect.x, histogramRect.y + histogramRect.height));
                Handles.DrawLine(new Vector3(histogramRect.x, histogramRect.y + histogramRect.height), new Vector3(histogramRect.x + histogramRect.width, histogramRect.y + histogramRect.height));

                float maxHours = dailyHours.Max();
                float xStep = histogramRect.width / daysInRange;
                float yStep = histogramRect.height / maxHours;

                Handles.color = Color.green;
                for (int i = 0; i < daysInRange; i++)
                {
                    var barRect = new Rect(histogramRect.x + i * xStep, histogramRect.y + histogramRect.height - dailyHours[i] * yStep, xStep - 2, dailyHours[i] * yStep);
                    Handles.DrawSolidRectangleWithOutline(barRect, Color.green, Color.black);
                }
            }
        }

        private void DisplayReports()
        {
            if (_timeMonitorData.years.Count == 0)
            {
                EditorGUILayout.LabelField("No session data available.");
                return;
            }

            GUILayout.Label("Weekly Report", EditorStyles.boldLabel);
            DisplayReportForPeriod(DateTime.Now.AddDays(-7), DateTime.Now);

            GUILayout.Space(20);

            GUILayout.Label("Monthly Report", EditorStyles.boldLabel);
            DisplayReportForPeriod(DateTime.Now.AddMonths(-1), DateTime.Now);
        }

        private void DisplayReportForPeriod(DateTime startDate, DateTime endDate)
        {
            var data = GetSessionDataInRange(startDate, endDate);
            if (data == null)
            {
                EditorGUILayout.LabelField("No data available for the selected period.");
                return;
            }

            float[] dailyHours = data.Item1;
            float totalHours = dailyHours.Sum();
            float averageDailyHours = dailyHours.Average();
            float maxDailyHours = dailyHours.Max();
            float minDailyHours = dailyHours.Min();

            GUILayout.Label($"Total Hours: {totalHours:F2}");
            GUILayout.Label($"Average Daily Hours: {averageDailyHours:F2}");
            GUILayout.Label($"Max Daily Hours: {maxDailyHours:F2}");
            GUILayout.Label($"Min Daily Hours: {minDailyHours:F2}");
        }

        private void DisplayAnalysis()
        {
            if (_timeMonitorData.years.Count == 0)
            {
                EditorGUILayout.LabelField("No session data available.");
                return;
            }

            GUILayout.Label("Trend Analysis", EditorStyles.boldLabel);
            DisplayTrendAnalysis(DateTime.Now.AddMonths(-1), DateTime.Now);
        }

        private void DisplayTrendAnalysis(DateTime startDate, DateTime endDate)
        {
            var data = GetSessionDataInRange(startDate, endDate);
            if (data == null)
            {
                EditorGUILayout.LabelField("No data available for the selected period.");
                return;
            }

            float[] dailyHours = data.Item1;
            int daysInRange = data.Item2;

            GUILayout.Label("Trend of Hours Worked per Day", EditorStyles.boldLabel);

            Rect graphRect = GUILayoutUtility.GetRect(600, 300);
            if (Event.current.type == EventType.Repaint)
            {
                Handles.color = Color.black;
                Handles.DrawLine(new Vector3(graphRect.x, graphRect.y), new Vector3(graphRect.x, graphRect.y + graphRect.height));
                Handles.DrawLine(new Vector3(graphRect.x, graphRect.y + graphRect.height), new Vector3(graphRect.x + graphRect.width, graphRect.y + graphRect.height));

                float maxHours = dailyHours.Max();
                float xStep = graphRect.width / daysInRange;
                float yStep = graphRect.height / maxHours;

                Handles.color = Color.blue;
                for (int i = 0; i < daysInRange - 1; i++)
                {
                    var start = new Vector3(graphRect.x + i * xStep, graphRect.y + graphRect.height - dailyHours[i] * yStep);
                    var end = new Vector3(graphRect.x + (i + 1) * xStep, graphRect.y + graphRect.height - dailyHours[i + 1] * yStep);
                    Handles.DrawLine(start, end);

                    Handles.DrawSolidDisc(start, Vector3.forward, 2);
                }

                Handles.color = Color.red;
                for (int i = 0; i < daysInRange; i++)
                {
                    var point = new Vector3(graphRect.x + i * xStep, graphRect.y + graphRect.height - dailyHours[i] * yStep);
                    Handles.DrawSolidDisc(point, Vector3.forward, 2);
                }
            }
        }

        private static bool IsCodeEditorActive(string editorProcessName)
        {
            Process[] processes = Process.GetProcessesByName(editorProcessName);
            return processes.Length > 0 && processes[0].MainWindowTitle.Length > 0;
        }

        private void Update()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorApplication.isPlaying)
            {
                if (_selectedCodeEditor != "None" && IsCodeEditorActive(GetProcessName(_selectedCodeEditor)))
                {
                    _currentSession.endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    EditorUtility.SetDirty(_timeMonitorData);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private static string GetProcessName(string editor)
        {
            return editor switch
            {
                    "Visual Studio" => "devenv",
                    "Rider" => "rider",
                    "VS Code" => "code",
                    _ => string.Empty
            };
        }

        private sealed class SettingsWindow : EditorWindow
        {
            public static void ShowSettingWindow()
            {
                var window = GetWindow<SettingsWindow>("Settings");
                window.minSize = new Vector2(250, 200);
            }

            private void OnGUI()
            {
                GUILayout.Label("Settings", EditorStyles.boldLabel);

                if (GUILayout.Button("Combine All Sessions"))
                {
                    CombineAllSessions();
                }

                if (GUILayout.Button("Clear All Data"))
                {
                    ClearAllData();
                }

                GUILayout.Space(20);

                GUILayout.Label("Select Code Editor", EditorStyles.boldLabel);

                int selectedCodeEditorIndex = GetCodeEditorIndex();

                selectedCodeEditorIndex = EditorGUILayout.Popup("Code Editor", selectedCodeEditorIndex, new[] { "None", "Visual Studio", "Rider", "VS Code" });

                _selectedCodeEditor = new[] { "None", "Visual Studio", "Rider", "VS Code" }[selectedCodeEditorIndex];

                if (GUI.changed)
                {
                    EditorPrefs.SetString("SelectedCodeEditor", _selectedCodeEditor);
                }
            }

            private void CombineAllSessions()
            {
                if (_timeMonitorData == null)
                {
                    LoadOrCreateData();
                }

                foreach (Year year in _timeMonitorData.years)
                {
                    foreach (Month month in year.months)
                    {
                        foreach (Day day in month.days)
                        {
                            if (day.sessions.Count > 1)
                            {
                                var combinedStartTime = DateTime.MaxValue;
                                var combinedEndTime = DateTime.MinValue;

                                foreach (Session session in day.sessions)
                                {
                                    DateTime startTime = DateTime.Parse(session.startTime);
                                    DateTime endTime = DateTime.Parse(session.endTime);

                                    if (startTime < combinedStartTime) combinedStartTime = startTime;
                                    if (endTime > combinedEndTime) combinedEndTime = endTime;
                                }

                                day.sessions.Clear();
                                day.sessions.Add(new Session
                                {
                                    startTime = combinedStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    endTime = combinedEndTime.ToString("yyyy-MM-dd HH:mm:ss")
                                });
                            }
                        }
                    }
                }

                EditorUtility.SetDirty(_timeMonitorData);
                AssetDatabase.SaveAssets();
            }

            private static void ClearAllData()
            {
                if (_timeMonitorData != null)
                {
                    _timeMonitorData.years.Clear();
                    EditorUtility.SetDirty(_timeMonitorData);
                    AssetDatabase.SaveAssets();
                }
            }

            private static int GetCodeEditorIndex()
            {
                string currentEditor = EditorPrefs.GetString("SelectedCodeEditor", "None");
                return currentEditor switch
                {
                        "Visual Studio" => 1,
                        "Rider" => 2,
                        "VS Code" => 3,
                        _ => 0
                };
            }
        }

        private Tuple<float[], int> GetSessionDataInRange(DateTime startDate, DateTime endDate)
        {
            if (_timeMonitorData.years.Count == 0)
            {
                return null;
            }

            var dailyHoursDict = new SortedDictionary<DateTime, float>();

            foreach (Year year in _timeMonitorData.years)
            {
                foreach (Month month in year.months)
                {
                    foreach (Day day in month.days)
                    {
                        var date = new DateTime(year.yearNumber, month.monthNumber, day.dayNumber);
                        if (date >= startDate && date <= endDate)
                        {
                            float totalDayTime = 0;
                            foreach (Session session in day.sessions)
                            {
                                DateTime start = DateTime.Parse(session.startTime);
                                DateTime end = DateTime.Parse(session.endTime);
                                totalDayTime += (float)(end - start).TotalHours;
                            }
                            dailyHoursDict[date] = totalDayTime;
                        }
                    }
                }
            }

            int daysInRange = (endDate - startDate).Days + 1;
            float[] dailyHours = new float[daysInRange];
            for (int i = 0; i < daysInRange; i++)
            {
                DateTime date = startDate.AddDays(i);
                dailyHours[i] = dailyHoursDict.ContainsKey(date) ? dailyHoursDict[date] : 0;
            }

            return new Tuple<float[], int>(dailyHours, daysInRange);
        }
    }
}
