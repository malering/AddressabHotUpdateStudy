using System;
using System.Collections.Generic;

    public class ByteTransferHelper
    {
        private static Dictionary<string, double> _dict = new Dictionary<string, double>();

        public static double GetMB(long kSize)
        {
            ByteConversionGBMBKB(kSize);
            var d = _dict["MB"];
            return d;
        }

        /// <summary>
        /// byte转换为GB/MB/KB
        /// </summary>
        /// <param name="kSize"></param>
        /// <returns></returns>
        public static void ByteConversionGBMBKB(long kSize)
        {
            _dict.Clear();
            int GB = 1024 * 1024 * 1024; //定义GB的计算常量
            int MB = 1024 * 1024; //定义MB的计算常量
            int KB = 1024; //定义KB的计算常量

            if (kSize / GB >= 1) //如果当前Byte的值大于等于1GB
            {
                _dict.Add("GB", Math.Round(kSize / (float)GB, 2)); //将其转换成GB
            }
            else if (kSize / MB >= 1) //如果当前Byte的值大于等于1MB
            {
                _dict.Add("MB", Math.Round(kSize / (float)MB, 2)); //将其转换成MB
            }
            else if (kSize / KB >= 1) //如果当前Byte的值大于等于1KB
            {
                _dict.Add("KB", Math.Round(kSize / (float)KB, 2)); //将其转换成KB
            }
            else
            {
                _dict.Add("Byte", kSize); //显示Byte值
            }
        }
    }
