using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R123.View
{
    public class VolumeController : Encoder
    {
        public event DelegateChangeValue ValueChanged;
        public VolumeController(System.Windows.Controls.Image image) :
            base(image, 100)
        {
        }
        public new decimal Value => Convert.ToDecimal(base.Value) / maxValue;
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();

            System.Diagnostics.Trace.WriteLine($"Volume = {base.Value}; ");
        }
    }
}