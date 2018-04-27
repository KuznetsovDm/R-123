using R123.Radio.Model;

namespace RadioTask.Model.RadioContexts.Info
{
    public class FixFrequencyParameter
    {
        public SubFrequencyState SubFrequency { get; set; }
        public double Frequency { get; set; }
        public RangeState Range { get; set; }
    }
}
