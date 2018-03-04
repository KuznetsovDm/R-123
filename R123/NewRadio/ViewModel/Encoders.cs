using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace R123.NewRadio.ViewModel
{
    class Frequency : View.ContinuouslyRotating
    {
        public Frequency()
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/FrequencyControl.gif", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateFrequency"));
            mouseMoveFactor = 0.25;
            changeAngleMouseWheel = 360 / 15.75 * 0.0025;
        }
        public override void TestRequestRotateCommand(ICommand newValue)
        {
            newValue.CanExecuteChanged += (object s, EventArgs e) =>
            {
                if (e is CommandEventArgs)
                    canRotate = (e as CommandEventArgs).CanExecute;
            };
        }
    }

    class Volume : View.ContinuouslyRotating
    {
        public Volume()
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/VolumeControl.gif", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateVolume"));
            changeAngleMouseWheel = 360 / 1 * 0.1;
        }
    }

    class Noise : View.ContinuouslyRotating
    {
        public Noise()
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/NoiseVolumeControl.gif", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateNoise"));
            changeAngleMouseWheel = 360 / 0.9 * 0.1;
        }
    }

    class Antenna : View.ContinuouslyRotating
    {
        public Antenna()
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/AntennaControl.png", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateAntenna"));
            SetBinding(RequestChangeMouseMoveFactor, new Binding("AntennaFixerAngle"));
            MouseMoveFactor = 0;
        }
        public override void TestRequestRotateCommand(ICommand newValue)
        {
            newValue.CanExecuteChanged += (object s, EventArgs e) =>
            {
                if (e is CommandEventArgs)
                    canRotate = (e as CommandEventArgs).CanExecute;
            };
        }

        protected override double CheckingAngle(double angle)
        {
            return (angle + 360) % 360;
        }

        public double MouseMoveFactor
        {
            get => mouseMoveFactor;
            set
            {
                if (value < 0 || value > 1)
                    throw new IndexOutOfRangeException("MouseMoveFactor");

                mouseMoveFactor = value;
            }
        }

        #region dp double mouseMoveFactor
        public double RequestChangeMouseMoveFactorCommand
        {
            get { return (double)GetValue(RequestRotateCommandProperty); }
            set { SetValue(RequestRotateCommandProperty, value); }
        }

        public static readonly DependencyProperty RequestChangeMouseMoveFactor =
            DependencyProperty.Register("RequestChangeMouseMoveFactorCommand", 
                                        typeof(double),
                                        typeof(Antenna), 
                                        new FrameworkPropertyMetadata(360.0, OnMouseMoveFactorChanged));

        private static void OnMouseMoveFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Antenna o = d as Antenna;
            o.MouseMoveFactor = 1 - (double)(e.NewValue) / 360;
        }
        #endregion
    }

    class AntennaFixer : View.ContinuouslyRotating
    {
        public AntennaFixer()
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/AntennaFixer.png", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateAntennaFixer"));
            changeAngleMouseWheel = 360 / 1 * 0.1;
        }

        #region dp double AntennaAngle
        public static readonly DependencyProperty AntennaAngleProperty = DependencyProperty.Register(
            "AntennaAngle",
            typeof(double),
            typeof(AntennaFixer),
            new UIPropertyMetadata(0.0,
                new PropertyChangedCallback(AntennaAngleChanged)));

        public double AntennaAngle
        {
            get { return (double)GetValue(AntennaAngleProperty); }
            set { SetValue(AntennaAngleProperty, value); }
        }

        private static void AntennaAngleChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            AntennaFixer image = (AntennaFixer)depObj;
            image.DefaultAngle = Convert.ToDouble(args.NewValue);
        }

        public double DefaultAngle
        {
            set => DefaultRotateTransform.Angle = value;
        }
        #endregion
    }

    class Clamp0 : View.ContinuouslyRotating
    {
        public Clamp0() : base(90)
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/FixedFrequencySwitcherScrew.gif", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateClamp0"));
            changeAngleMouseWheel = 360 / 1 * 0.1;
            DefaultRotateTransform.Angle = 90;
        }
    }
    class Clamp1 : View.ContinuouslyRotating
    {
        public Clamp1() : base(90)
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/FixedFrequencySwitcherScrew.gif", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateClamp1"));
            changeAngleMouseWheel = 360 / 1 * 0.1;
        }
    }
    class Clamp2 : View.ContinuouslyRotating
    {
        public Clamp2() : base(90)
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/FixedFrequencySwitcherScrew.gif", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateClamp2"));
            changeAngleMouseWheel = 360 / 1 * 0.1;
            DefaultRotateTransform.Angle = 90;
        }
    }
    class Clamp3 : View.ContinuouslyRotating
    {
        public Clamp3() : base(90)
        {
            TheImage.Source = new BitmapImage(new Uri("/Files/Images/FixedFrequencySwitcherScrew.gif", UriKind.Relative));
            SetBinding(RequestRotateCommandProperty, new Binding("RequestRotateClamp3"));
            changeAngleMouseWheel = 360 / 1 * 0.1;
        }
    }
}
