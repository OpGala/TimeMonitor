using System.Collections.Generic;
using UnityEngine;

namespace TimeMonitor.Data
{
      [System.Serializable]
      public class Session
      {
            public string startTime;
            public string endTime;
      }

      [System.Serializable]
      public class Day
      {
            public int dayNumber;
            public List<Session> sessions = new List<Session>();
      }

      [System.Serializable]
      public class Month
      {
            public int monthNumber;
            public List<Day> days = new List<Day>();
      }

      [System.Serializable]
      public class Year
      {
            public int yearNumber;
            public List<Month> months = new List<Month>();
      }

      [CreateAssetMenu(fileName = "TimeMonitorData", menuName = "TimeMonitor/Data", order = 1)]
      public sealed class TimeMonitorData : ScriptableObject
      {
            public int startYear;
            public List<Year> years = new List<Year>();
      }
}