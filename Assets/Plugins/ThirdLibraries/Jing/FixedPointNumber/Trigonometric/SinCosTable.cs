using System.Collections.Generic;

namespace Jing.FixedPointNumber
{
    /// <summary>
    /// Sin和Cos计算的查找表生成工具
    /// </summary>
    public static class SinCosTable
    {
        static Number[] _sinTable;

        static Number[] _cosTable;

        static SinCosTable()
        {
            BuildTable();
        }

        static void BuildTable()
        {
            List<Number> sinList = new List<Number>();
            List<Number> cosList = new List<Number>();
            for (int degree = 0; degree < 360; degree++)
            {
                var radians = degree * TrigonometricDefine.Deg2Rad;
                var sin = System.Math.Sin(radians);
                sinList.Add(Number.CreateFromDouble(sin));
                var cos = System.Math.Cos(radians);
                cosList.Add(Number.CreateFromDouble(cos));
            }
            _sinTable = sinList.ToArray();
            _cosTable = cosList.ToArray();
        }

        /// <summary>
        /// 获取正弦查找表
        /// </summary>
        /// <returns></returns>
        public static Number[] GetSinTable()
        {
            var sinTable = new Number[_sinTable.Length];
            _sinTable.CopyTo(sinTable, 0);
            return sinTable;
        }

        /// <summary>
        /// 获取余弦查找表
        /// </summary>
        /// <returns></returns>
        public static Number[] GetCosTable()
        {
            var cosTable = new Number[_cosTable.Length];
            _cosTable.CopyTo(cosTable, 0);
            return cosTable;
        }

        #region sin
        /// <summary>
        /// 通过角度计算Sin值
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Number SinByDegree(Number degree)
        {
            degree %= 360;
            var idx = Math.Round(degree);
            return _sinTable[idx.ToInt()];
        }

        /// <summary>
        /// 通过弧度计算Sin值
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Number SinByRadian(Number radian)
        {
            var degree = radian * Math.Rad2Deg;
            return SinByDegree(degree);
        }
        #endregion

        #region cos

        /// <summary>
        /// 通过角度计算Cos值
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Number CosByDegree(Number degree)
        {
            degree %= 360;
            var idx = Math.Round(degree);
            return _cosTable[idx.ToInt()];
        }

        /// <summary>
        /// 通过弧度计算Cos值
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Number CosByRadian(Number radian)
        {
            var degree = radian * Math.Rad2Deg;
            return CosByDegree(degree);
        }
        #endregion

    }
}
