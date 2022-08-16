using System;
using System.Collections.Generic;

public enum ByteUnitType
{
    BYTE,
    KB,
    MB,
    GB,
}

    public class ByteTransferHelper
    {
        public static double GetMB(long kSize, ByteUnitType byteUnitType)
        {
            if (kSize == 0) return 0;
            var d = ByteConversionGBMBKB(kSize, byteUnitType);
            return d;
        }

        /// <summary>
        /// byte转换为GB/MB/KB
        /// </summary>
        /// <param name="kSize"></param>
        /// <returns></returns>
        public static double ByteConversionGBMBKB(long kSize, ByteUnitType byteUnitType)
        {
            switch (byteUnitType)
            {
                case ByteUnitType.BYTE:
                    return kSize;
                case ByteUnitType.KB:
                    int KB = 1024; //定义KB的计算常量
                    return Math.Round(kSize / (float) KB, 2);
                case ByteUnitType.MB:
                    int MB = 1024 * 1024; //定义MB的计算常量
                    return Math.Round(kSize / (float) MB, 2);
                case ByteUnitType.GB:
                    int GB = 1024 * 1024 * 1024; //定义GB的计算常量
                    return Math.Round(kSize / (float) GB, 2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(byteUnitType), byteUnitType, null);
            }
        }
    }
