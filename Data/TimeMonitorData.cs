using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimeMonitor
{
      [CreateAssetMenu(fileName = "TimeMonitorData", menuName = "TimeMonitor/TimeMonitorData")]
      public sealed class TimeMonitorData : ScriptableObject
      {
            public double TotalTime;
            public List<TimeData> Sessions = new List<TimeData>();
      }

      [Serializable]
      public class TimeData
      {
            public string StartDate;
            public string EndDate;
            public double SessionTime;
      }
}