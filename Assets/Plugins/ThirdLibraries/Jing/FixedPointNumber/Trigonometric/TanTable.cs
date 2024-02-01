using System.Collections.Generic;

namespace Jing.FixedPointNumber
{
    /// <summary>
    /// Tan计算的查找表生成工具
    /// </summary>
    public static class TanTable
    {
        static Number[] _tanTable;

        static TanTable()
        {
            BuildTable();
        }

        public static void BuildTable()
        {
            List<Number> list = new List<Number>();
            for (int degree = 0; degree < 360; degree++)
            {
                var radians = degree * TrigonometricDefine.Deg2Rad;
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
