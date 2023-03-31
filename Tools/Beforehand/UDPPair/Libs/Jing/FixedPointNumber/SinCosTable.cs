using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jing.FixedPointNumber
{
    /// <summary>
    /// Sin和Cos计算的查找表生成工具
    /// </summary>
    public static class SinCosTable
    {
        /// <summary>
        /// 生成的点数，如果过大，会因为浮点数转换问题导致Sin(90)值不是1但是无限接近1的情况。
        /// 720是个比较合适的数值
        /// </summary>
        const short ACCURACY = 720;

        static Number[] _sinTable;

        static Number[] _cosTable;

        static SinCosTable()
        {
            BuildTable(ACCURACY);
        }

        public static void BuildTable(short accuracy)
        {
            const double DOUBLE_PI = System.Math.PI * 2;
            double radianScale = DOUBLE_PI / accuracy;

            _sinTable = new Number[accuracy];
            _cosTable = new Number[accuracy];

            for (int i = 0; i < accuracy; i++)
            {
                var sin = System.Math.Sin(radianScale * i);
                _sinTable[i] = new Number(sin);

                var cos = System.Math.Cos(radianScale * i);
                _cosTable[i] = new Number(cos);
            }            
        }

        #region sin
        /// <summary>
        /// 构建SIN快速查找表
        /// </summary>
        /// <param name="accuracy">精度值，表示将2π分成多少份</param>
        /// <returns></returns>
        public static void BuildSinTable(short accuracy)
        {
            const double DOUBLE_PI = System.Math.PI * 2;
            double radianScale = DOUBLE_PI / accuracy;

            Number[] table = new Number[accuracy];
            for (int i = 0; i < table.Length; i++)
            {
                var result = System.Math.Sin(radianScale * i);
                table[i] = new Number(result);
            }

            _sinTable = table;
        }

        /// <summary>
        /// 通过角度计算Sin值
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Number SinByDegree(Number degree)
        {
            degree %= 360;
            var radian = degree * Math.RadianCoefficient;
            return SinByRadian(radian);
        }

        /// <summary>
        /// 通过弧度计算Sin值
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Number SinByRadian(Number radian)
        {
            radian %= Math.TwoPi;

            var radianScale = Math.TwoPi / _sinTable.Length;
            var index = (radian / radianScale).ToInt();
            var value = _sinTable[index];
            return value;
        }
        #endregion

        #region cos
        /// <summary>
        /// 构建SIN快速查找表
        /// </summary>
        /// <param name="accuracy">精度值，表示将2π分成多少份</param>
        /// <returns></returns>
        public static void BuildCosTable(short accuracy)
        {
            const double TWO_PI = System.Math.PI * 2;
            double radianScale = TWO_PI / accuracy;

            Number[] table = new Number[accuracy];
            for (int i = 0; i < table.Length; i++)
            {
                var result = System.Math.Cos(radianScale * i);
                result = System.Math.Round(result, 4);
                table[i] = new Number(result);
            }

            _cosTable = table;
        }

        /// <summary>
        /// 通过角度计算Sin值
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Number CosByDegree(Number degree)
        {
            degree %= Math.Degree360;
            var radian = degree * Math.RadianCoefficient;
            return CosByRadian(radian);
        }

        /// <summary>
        /// 通过弧度计算Sin值
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Number CosByRadian(Number radian)
        {
            radian %= Math.TwoPi;
            var radianScale = Math.TwoPi / _sinTable.Length;
            var index = (radian / radianScale).ToInt();
            var value = _cosTable[index];
            return value;
        }
        #endregion

    }
}
