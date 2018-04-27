namespace R123.Utils.RadioStation
{
    public static class BadFrequency
    {
        public static double[] Frequency { get; private set; } = { 213.0, 222.25, 222.5, 236.0, 236.25, 236.5, 238.25, 275.25, 287.0, 287.25, 296.25, 296.5, 313.52, 314.75, 315.0, 315.25, 323.75,
            364.75, 370.5, 379.75, 380.0, 393.5, 393.75, 394.0, 395.75, 432.0, 444.5, 444.75, 453.75, 454.0, 454.5, 472.25, 472.5, 472.75, 474.75, 512.0};

        public static bool ContainInRange(this double[] array, double value,double range)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].CompareInRange(value, range))
                    return true;
            }
            return false;
        }
    }
}
