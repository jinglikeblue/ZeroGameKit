using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Profiling;

namespace Zero
{
    /// <summary>
    /// 性能分析工具
    /// </summary>
    public static class PerformanceAnalysis
    {
        public static bool IsEnable { get; private set; } = false;

        public static Dictionary<string, AnalysisRecord> _analysisRecordDic;

        static PerformanceAnalysis()
        {
            _analysisRecordDic = new Dictionary<string, AnalysisRecord>();
        }

        public static void SetEnable(bool enable)
        {
            if (IsEnable == enable)
            {
                return;
            }
            IsEnable = enable;
        }

        #region 分析记录
        public class AnalysisRecord
        {
            public double maxCostTime { get; private set; } = 0;

            public double showLimit = 1;

            Stopwatch sw = new Stopwatch();

            public string name { get; private set; }

            public AnalysisRecord(string name)
            {
                this.name = name;
            }

            public void Start()
            {
                sw.Reset();
                sw.Start();
            }

            public double End()
            {
                sw.Stop();
                var t = sw.Elapsed.TotalMilliseconds;
                if (t > maxCostTime)
                {
                    maxCostTime = t;
                }
                return t;
            }

            public void Clean()
            {
                maxCostTime = -1;
            }

            public override string ToString()
            {
                return $"{name}:{maxCostTime} ms";
            }
        }

        #endregion

        /// <summary>
        /// 开始分析
        /// </summary>
        /// <param name="name"></param>
        /// <param name="showLimit"></param>
        public static void BeginAnalysis(string name, double showLimit = 1)
        {
            if (false == IsEnable)
            {
                return;
            }
            Profiler.BeginSample(name);


            if (false == _analysisRecordDic.ContainsKey(name))
            {
                var record = new AnalysisRecord(name);
                lock (_analysisRecordDic)
                {                   
                    _analysisRecordDic.Add(name, record);
                }
                _analysisRecordDic[name].showLimit = showLimit;
            }

            _analysisRecordDic[name].Start();
        }

        /// <summary>
        /// 结束分析
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static double EndAnalysis(string name)
        {
            if (false == IsEnable)
            {
                return -1;
            }

            var currentCost = _analysisRecordDic[name].End();
            Profiler.EndSample();
            return currentCost;
        }

        public static AnalysisRecord[] GetAnalysisRecords()
        {
            if (false == IsEnable)
            {
                return new AnalysisRecord[0];
            }
            return _analysisRecordDic.Values.ToArray();
        }

        public static AnalysisRecord GetAnalysisRecord(string name)
        {
            if (false == _analysisRecordDic.ContainsKey(name))
            {
                return null;
            }

            return _analysisRecordDic[name];
        }
    }
}
