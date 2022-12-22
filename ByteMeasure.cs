using System;

namespace HS.Utils
{
    public static class ByteMeasure
    {
        /// <summary>
        /// 바이트 단위입니다
        /// </summary>                      
        public enum ByteUnit { Byte, KByte, MByte, GByte, TByte, PByte, OutByte }

        /// <summary>
        /// 바이트 상태를 나눠줍니다.
        /// </summary>
        /// <param name="Byte">네트워크 바이트 입니다.</param>
        /// <param name="Dec">true면 1000으로 계산하고 false면 1024으로 계산합니다.</param>
        /// <param name="Measure">바이트 현재 상태를 반환합니다.</param>
        /// <returns<</returns<
        public static double MeasureCaculate(this long Byte, bool Dec, out ByteUnit Measure)
        {
            if (Dec)
            {
                if (Byte < 1000) { Measure = ByteUnit.Byte; return (double)Byte; }
                else if (Byte < 1000000) { Measure = ByteUnit.KByte; return Byte / 1000; }
                else if (Byte < 1000000000) { Measure = ByteUnit.MByte; return Byte / 1000000; }
                else if (Byte < 1000000000000) { Measure = ByteUnit.GByte; return (Byte / 1000000000); }
                else if (Byte < 1000000000000000) { Measure = ByteUnit.TByte; return Byte / 1000000000000; }
                else if (Byte < 1000000000000000000) { Measure = ByteUnit.PByte; return Byte / 1000000000000000; }
                else { Measure = ByteUnit.OutByte; return -1; }
            }
            else
            {
                if (Byte < 1024) { Measure = ByteUnit.Byte; return (double)Byte; }
                else if (Byte < 1048576) { Measure = ByteUnit.KByte; return Byte / 1024; }
                else if (Byte < 1073741824) { Measure = ByteUnit.MByte; return Byte / 1048576; }
                else if (Byte < 1099511627776) { Measure = ByteUnit.GByte; return Byte / 1073741824; }
                else if (Byte < 1125899906842624) { Measure = ByteUnit.TByte; return Byte / 1099511627776; }
                else if (Byte < 1152921504606846976) { Measure = ByteUnit.PByte; return Byte / 1125899906842624; }
                else { Measure = ByteUnit.OutByte; return -1; }
            }
        }
        /// 바이트 상태를 나눠줍니다.
        /// </summary>
        /// <param name="Byte">네트워크 바이트 입니다.</param>
        /// <param name="Dec">true면 1000으로 계산하고 false면 1024으로 계산합니다.</param>
        /// <param name="최대자릿수">지정한 자릿수까지만 표시됩니다.</param>
        /// <param name="반올림">자릿수를 올릴때 반올림을 할지 결정합니다.</param>
        /// <param name="Measure">바이트 현재 상태를 반환합니다.</param>
        /// <returns<</returns<
        public static double MeasureCaculate(this long Byte, bool Dec, byte 최대자릿수, bool 반올림, out ByteUnit Measure)
        {
            if (Dec)
            {
                if (Byte < 1000) { Measure = ByteUnit.Byte; return (double)Byte; }
                else if (Byte < 1000000) { Measure = ByteUnit.KByte; return Convert.ToDouble((Byte / 1000).ToString("N" + 최대자릿수)); }
                else if (Byte < 1000000000) { Measure = ByteUnit.MByte; return Convert.ToDouble((Byte / 1000000).ToString("N" + 최대자릿수)); }
                else if (Byte < 1000000000000) { Measure = ByteUnit.GByte; return Convert.ToDouble((Byte / 1000000000).ToString("N" + 최대자릿수)); }
                else if (Byte < 1000000000000000) { Measure = ByteUnit.TByte; return Convert.ToDouble((Byte / 1000000000000).ToString("N" + 최대자릿수)); }
                else if (Byte < 1000000000000000000) { Measure = ByteUnit.PByte; return Convert.ToDouble((Byte / 1000000000000000).ToString("N" + 최대자릿수)); }
                else { Measure = ByteUnit.OutByte; return -1; }
            }
            else
            {
                if (Byte < 1024) { Measure = ByteUnit.Byte; return (double)Byte; }
                else if (Byte < 1048576) { Measure = ByteUnit.KByte; return Convert.ToDouble((Byte / 1024).ToString("N" + 최대자릿수)); ; }
                else if (Byte < 1073741824) { Measure = ByteUnit.MByte; return Convert.ToDouble((Byte / 1048576).ToString("N" + 최대자릿수)); ; }
                else if (Byte < 1099511627776) { Measure = ByteUnit.GByte; return Convert.ToDouble((Byte / 1073741824).ToString("N" + 최대자릿수)); ; }
                else if (Byte < 1125899906842624) { Measure = ByteUnit.TByte; return Convert.ToDouble((Byte / 1099511627776).ToString("N" + 최대자릿수)); ; }
                else if (Byte < 1152921504606846976) { Measure = ByteUnit.PByte; return Convert.ToDouble((Byte / 1125899906842624).ToString("N" + 최대자릿수)); ; }
                else { Measure = ByteUnit.OutByte; return -1; }
            }
        }

        /// <summary<
        /// 바이트 상태를 나눠줍니다.
        /// </summary>
        /// <param name="Byte">네트워크 바이트 입니다.</param>
        /// <param name="Dec">true면 1000으로 계산하고 false면 1024으로 계산합니다.</param>
        /// <param name="Measure">바이트 현재 상태를 반환합니다.</param>
        /// <returns<</returns<
        public static double MeasureCaculate(this double Byte, bool Dec, out ByteUnit Measure)
        {
            if (Dec)
            {
                if (Byte < 1000) { Measure = ByteUnit.Byte; return (double)Byte; }
                else if (Byte < 1000000) { Measure = ByteUnit.KByte; return Byte / 1000; }
                else if (Byte < 1000000000) { Measure = ByteUnit.MByte; return Byte / 1000000; }
                else if (Byte < 1000000000000) { Measure = ByteUnit.GByte; return (Byte / 1000000000); }
                else if (Byte < 1000000000000000) { Measure = ByteUnit.TByte; return Byte / 1000000000000; }
                else if (Byte < 1000000000000000000) { Measure = ByteUnit.PByte; return Byte / 1000000000000000; }
                else { Measure = ByteUnit.OutByte; return -1; }
            }
            else
            {
                if (Byte < 1024) { Measure = ByteUnit.Byte; return (double)Byte; }
                else if (Byte < 1048576) { Measure = ByteUnit.KByte; return Byte / 1024; }
                else if (Byte < 1073741824) { Measure = ByteUnit.MByte; return Byte / 1048576; }
                else if (Byte < 1099511627776) { Measure = ByteUnit.GByte; return Byte / 1073741824; }
                else if (Byte < 1125899906842624) { Measure = ByteUnit.TByte; return Byte / 1099511627776; }
                else if (Byte < 1152921504606846976) { Measure = ByteUnit.PByte; return Byte / 1125899906842624; }
                else { Measure = ByteUnit.OutByte; return -1; }
            }
        }

        /// <summary<
        /// 바이트 상태를 나눠줍니다.
        /// </summary>
        /// <param name="Byte">네트워크 바이트 입니다.</param>
        /// <param name="Dec">true면 1000으로 계산하고 false면 1024으로 계산합니다.</param>
        /// <param name="최대자릿수">지정한 자릿수까지만 표시됩니다.</param>
        /// <param name="Round">자릿수를 올릴떄 반올림을 할지 결정합니다.</param>
        /// <param name="Measure">바이트 현재 상태를 반환합니다.</param>
        /// <returns<</returns<
        public static double MeasureCaculate(this double Byte, bool Dec, byte 최대자릿수/*, bool Round*/, out ByteUnit Measure)
        {
            if (Dec)
            {
                if (Byte < 1000) { Measure = ByteUnit.Byte; return (double)Byte; }
                else if (Byte < 1000000) { Measure = ByteUnit.KByte; return Math.Round(Byte / 1000, 최대자릿수); }
                else if (Byte < 1000000000) { Measure = ByteUnit.MByte; return Math.Round(Byte / 1000000, 최대자릿수); }
                else if (Byte < 1000000000000) { Measure = ByteUnit.GByte; return Math.Round(Byte / 1000000000, 최대자릿수); }
                else if (Byte < 1000000000000000) { Measure = ByteUnit.TByte; return Math.Round(Byte / 1000000000000, 최대자릿수); ; }
                else if (Byte < 1000000000000000000) { Measure = ByteUnit.PByte; return Math.Round(Byte / 1000000000000000, 최대자릿수); }
                else { Measure = ByteUnit.OutByte; return -1; }
            }
            else
            {
                if (Byte < 1024) { Measure = ByteUnit.Byte; return (double)Byte; }
                else if (Byte < 1048576) { Measure = ByteUnit.KByte; return Math.Round(Byte / 1024, 최대자릿수); }
                else if (Byte < 1073741824) { Measure = ByteUnit.MByte; return Math.Round(Byte / 1048576, 최대자릿수); }
                else if (Byte < 1099511627776) { Measure = ByteUnit.GByte; return Math.Round(Byte / 1073741824, 최대자릿수); }
                else if (Byte < 1125899906842624) { Measure = ByteUnit.TByte; return Math.Round(Byte / 1099511627776, 최대자릿수); }
                else if (Byte < 1152921504606846976) { Measure = ByteUnit.PByte; return Math.Round(Byte / 1125899906842624, 최대자릿수); }
                else { Measure = ByteUnit.OutByte; return -1; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Byte">바이트 크기 입니다</param>
        /// <param name="Dec">true면 1000으로 계산하고 false면 1024으로 계산합니다.</param>
        /// <param name="Space">숫자와 문자 중간에 띄어쓰기를 할지 여부입니다.</param>
        /// <param name="DisplayFullMeasure">True면 'Byte' 를 표시하고 False 면 'B' 만 표시합니다.</param>
        /// <param name="최대자릿수">지정한 자릿수까지만 표시됩니다.</param>
        /// <returns></returns>
        public static string NetworkString(this long Byte, bool Dec, bool Space,bool DisplayFullMeasure = false, byte 최대자릿수 = 2)
        {
            ByteUnit nm = ByteUnit.OutByte;
            double a = MeasureCaculate(Byte, Dec, 최대자릿수, out nm);

            return string.Format(Space?"{0} {1}":"{0}{1}", a.ToString("0.00") ,ByteUnitString(nm, DisplayFullMeasure));
        }
        public static string ByteUnitString(this ByteUnit Measure, bool DisplayFullMeasure)
        {
            ByteUnit nm = Measure;
            switch (nm)
            {
                case ByteUnit.Byte: return DisplayFullMeasure ? "Byte" : "B";
                case ByteUnit.KByte: return DisplayFullMeasure ? "KByte" : "KB";
                case ByteUnit.MByte: return DisplayFullMeasure ? "MByte" : "MB";
                case ByteUnit.GByte: return DisplayFullMeasure ? "GByte" : "GB";
                case ByteUnit.TByte: return DisplayFullMeasure ? "TByte" : "TB";
                case ByteUnit.PByte: return DisplayFullMeasure ? "PByte" : "PB";
                default: return DisplayFullMeasure ? "oByte" : "oB";
            }
        }
    }
}