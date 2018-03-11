using System;

namespace R123.Utils
{
    public static class DoubleExtentions
    {
        public const double AcceptableRangeForFrequency = 0.002;

        public static bool CompareInRange(this double a, double b,double range)
        {
            return a.GetDelta(b) < range;
        }

        public static double GetDelta(this double currentValue, double otherValue)
        {
            return Math.Abs(currentValue - otherValue);
        }
    }
}
