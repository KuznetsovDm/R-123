using R123.Radio.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioTask.Model.RadioContexts.Info
{
    public class FixFrequencyParameter
    {
        public SubFrequencyState SubFrequency { get; set; }
        public double Frequency { get; set; }
        public RangeState Range { get; set; }
    }
}
