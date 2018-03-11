using System.Windows.Data;

namespace R123.Radio.ViewModel
{
    public class Power : View.Switch
    {
        public Power()
        {
            SetBinding(RequestChangeValueCommandProperty, new Binding("RequestChangePowerValue"));
        }
    }

    public class Scale : View.Switch
    {
        public Scale()
        {
            SetBinding(RequestChangeValueCommandProperty, new Binding("RequestChangeScaleValue"));
        }
    }

    public class SubFrequencySwitches0 : View.Switch
    {
        public SubFrequencySwitches0()
        {
            SetBinding(RequestChangeValueCommandProperty, new Binding("RequestChangeSubFrequency0Value"));
        }
    }

    public class SubFrequencySwitches1 : View.Switch
    {
        public SubFrequencySwitches1()
        {
            SetBinding(RequestChangeValueCommandProperty, new Binding("RequestChangeSubFrequency1Value"));
        }
    }

    public class SubFrequencySwitches2 : View.Switch
    {
        public SubFrequencySwitches2()
        {
            SetBinding(RequestChangeValueCommandProperty, new Binding("RequestChangeSubFrequency2Value"));
        }
    }

    public class SubFrequencySwitches3 : View.Switch
    {
        public SubFrequencySwitches3()
        {
            SetBinding(RequestChangeValueCommandProperty, new Binding("RequestChangeSubFrequency3Value"));
        }
    }

    public class Tone : View.Tone
    {
        public Tone()
        {
            SetBinding(RequestChangeValueCommandProperty, new Binding("RequestChangeToneValue"));
        }
    }

    public class Tangent : View.Tangent
    {
        public Tangent()
        {
            SetBinding(RequestChangeValueCommandProperty, new Binding("RequestChangeTangentValue"));
        }
    }
}
