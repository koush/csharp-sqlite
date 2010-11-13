using System;


namespace Community.CsharpSqlite.WindowsPhone7
{
    public static class MissingMethods
    {
        public static long DoubleToInt64Bits(double value)
        {
            return BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
        }
        
        public static double Int64BitsToDouble(long value)
        {
            return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
        }
    }
}
