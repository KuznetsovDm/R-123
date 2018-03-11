using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace R123.Radio.ViewModel
{
    class Range : View.DiscretelyRotating
    {
        public Range() : base(new Vector(-1, 0), 6)
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/Range_switcher.gif", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateRange"));
        }
    }

    class Voltage : View.DiscretelyRotating
    {
        public Voltage() : base(new Vector(-13, -45), 12)
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/VoltageTesterArrow.png", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateVoltage"));
        }
    }

    class WorkMode : View.DiscretelyRotating
    {
        public WorkMode() : base(new Vector(11, 40), 
                                 Model.Converter.WorkMode.numberPosiotion,
                                 Model.Converter.WorkMode.maxAngle,
                                 Model.Converter.WorkMode.defaultAngle)
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/WorkModeChangerArrow.png", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateWorkMode"));
        }
    }
}
