using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jing.FixedPointNumber
{
    /// <summary>
    /// Tan计算的查找表生成工具
    /// </summary>
    public static class TanTable
    {
        public enum EAccuracy
        {
            /// <summary>
            /// 每1Degree为一个刻度
            /// </summary>
            NORMAL,

            /// <summary>
            /// 每0.5Degree为一个刻度
            /// </summary>
            HIGH
        }

        static Number[] _tanTable;

        const double Deg2Rad = 0.017453D;

        const double Rad2Deg = 57.29578D;

        static TanTable()
        {
            BuildTable();
        }

        public static void BuildTable()
        {
            List<Number> list = new List<Number>();
            for (int degree = 0; degree < 360; degree++)
            {
                var radians = degree * Deg2Rad;
                var tan = System.Math.Tan(radians);
                list.Add(Number.CreateFromDouble(tan));
            }
            _tanTable = list.ToArray();
        }

        /// <summary>
        /// 通过角度计算Tan值
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Number TanByDegree(Number degree)
        {
            degree %= 360;
            var idx = Math.Round(degree);
            return _tanTable[idx.ToInt()];            
        }

        /// <summary>
        /// 通过弧度计算Tan值
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Number TanByRadian(Number radian)
        {
            var degree = radian * Math.Rad2Deg;
            return TanByDegree(degree);
        }
    }
}
