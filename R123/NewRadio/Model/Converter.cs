using System;

namespace R123.NewRadio.Model
{
    public static class Converter
    {
        public static class Frequency
        {
            public const double minValue = 20;
            public const double maxValueRange = 35.75;
            public const double maxValue = 51.5;

            private static double k = 360 / (maxValueRange - minValue);

            public static double ToValue(double angle, int numberSubFrequency) =>
                (numberSubFrequency == 1) ? (angle / k + minValue) : (angle / k + maxValueRange);

            public static double ToAngle(double value) => (value - minValue) * k;

            public static bool OutOfRange(double value) => value < minValue || value > maxValue;
            public static bool FirstRangeOutOfRange(double value) => value < minValue || value > maxValueRange;
        }

        public static class Volume
        {
            public const double minValue = 0.1;
            public const double maxValue = 1;

            private static double k = 360 / (maxValue - minValue);

            public static double ToValue(double angle) => angle / k + minValue;
            public static double ToAngle(double value) => (value - minValue) * k;

            public static bool OutOfRange(double value) => value < minValue || value > maxValue;
        }

        public static class Noise
        {
            public const double minValue = 0.1;
            public const double maxValue = 1;

            private static double k = 360 / (maxValue - minValue);

            public static double ToValue(double angle) => 1 - angle / k;
            public static double ToAngle(double value) => value * k;

            public static bool OutOfRange(double value) => value < minValue || value > maxValue;
        }

        public static class Antenna
        {
            public const double minValue = 0;
            public const double maxValue = 1;

            public static double ToValue(double angleFrequency, double angleAntenna)
            {
                double difference = (angleAntenna - angleFrequency + 360) % 360;
                if (difference > 180) difference = 360 - difference;
                int numberHill = (int)(difference / 36);
                double maxValue = 1 - Math.Abs(numberHill * 0.2);
                double value = (Math.Cos(difference * Math.PI / 36) + 1) / 2 * maxValue;
                return value;
            }

            public static double ToAngle(double value) => value;
            public static bool OutOfRange(double value) => value < minValue || value > maxValue;
            public static bool AngleOutOfRange(double angle) => angle < 0 || angle > 360;
        }

        public static class AntennaFixer
        {
            public const double numberPosiotion = 3;

            public static ClampState ToValue(double angle)
            {
                if (angle == 0) return ClampState.Unfixed;
                else if (angle == 360) return ClampState.Fixed;
                else return ClampState.Medium;
            }
            public static double ToAngle(ClampState value)
            {
                if (value == ClampState.Fixed) return 360;
                else if (value == ClampState.Unfixed) return 0;
                else return 180;
            }

            public static bool OutOfRange(ClampState value) => value < 0 || (int)value >= numberPosiotion;
        }

        public static class Clamp
        {
            public const double numberPosiotion = 3;

            public static ClampState ToValue(double angle)
            {
                if (angle == 0) return ClampState.Unfixed;
                else if (angle == 90) return ClampState.Fixed;
                else return ClampState.Medium;
            }
            public static double ToAngle(ClampState value)
            {
                if (value == ClampState.Fixed) return 90;
                else if (value == ClampState.Unfixed) return 0;
                else return 45;
            }

            public static bool OutOfRange(ClampState value) => value < 0 || (int)value >= numberPosiotion;
        }

        public static class Range
        {
            public const int numberPosiotion = 6;

            private static double k = 360 / (double)numberPosiotion;

            public static RangeState ToValue(double angle) => (RangeState)(angle / k);
            public static double ToAngle(RangeState value) => (int)value * k;

            public static bool OutOfRange(RangeState value) => value < 0 || (int)value >= numberPosiotion;
        }

        public static class WorkMode
        {
            public const int numberPosiotion = 3;
            public const double maxAngle = 100;
            public const double defaultAngle = -39;

            private static double k = maxAngle / (numberPosiotion - 1);

            public static WorkModeState ToValue(double angle) => (WorkModeState)(angle / k);
            public static double ToAngle(WorkModeState value) => (int)value * k;

            public static bool OutOfRange(WorkModeState value) => value < 0 || (int)value >= numberPosiotion;
        }

        public static class Voltage
        {
            public const int numberPosiotion = 12;

            private static double k = 360 / numberPosiotion;

            public static VoltageState ToValue(double angle) => (VoltageState)(angle / k);
            public static double ToAngle(VoltageState value) => (int)value * k;

            public static bool OutOfRange(VoltageState value) => value < 0 || (int)value >= numberPosiotion;
        }

        public static class TurnedState
        {
            public const int numberPosiotion = 2;

            public static bool ToBoolean(Turned value) => value == Turned.On;
            public static Turned ToState(bool value) => value ? Turned.On : Turned.Off;

            public static bool OutOfRange(Turned value) => value < 0 || (int)value >= numberPosiotion;
        }

        public static class NumberSubFrequency
        {
            public const int numberPosiotion = 2;

            public static SubFrequencyState ToValue(int number) => (SubFrequencyState)(number - 1);

            public static bool OutOfRange(SubFrequencyState value) => value < 0 || (int)value >= numberPosiotion;
        }
    }
}
