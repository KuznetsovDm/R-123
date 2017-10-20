using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace R_123.View
{
    class PositionSwitcher : ImagesControl
    {
        private int maxValue = 20;
        private int currentValue = 0;
        protected double minAngle = 0;
        protected double maxAngle = 360;
        private double defAngle = 0;
        public PositionSwitcher(Image image, double defAngle = 0) :
            base(image)
        {
            this.defAngle = defAngle;
            image.MouseWheel += OnMouseWheel;
            image.MouseDown += Image_MouseDown;
        }
        public int Value => currentValue;

        private void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                CurrentValue += 1;
            else
                CurrentValue -= 1;
        }
        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            double mAngle = minAngle * System.Math.PI * 2 / 360;
            Options.canvas.MouseUp += Image_MouseUp;
            double left = Canvas.GetLeft(Image) + Image.Width / 2;
            double top = Canvas.GetTop(Image) + Image.Height / 2;
            double step = System.Math.PI * 2 * (maxAngle - minAngle) / 360 / maxValue;
            for (int i = 0; i < maxValue; i++)
            {
                double p1 = (i - 0.5 + defAngle) * step - mAngle;
                double p2 = (i + 0.5 + defAngle) * step - mAngle;
                double p1x = System.Math.Cos(p1) * 1500;
                double p1y = System.Math.Sin(p1) * 1500;
                double p2x = System.Math.Cos(p2) * 1500;
                double p2y = System.Math.Sin(p2) * 1500;

                PointCollection myPointCollection = new PointCollection
                {
                    new Point(left, top),
                    new Point(left+p1x, top+p1y),
                    new Point(left+p2x, top+p2y)
                };
                Polygon polygon = new Polygon
                {
                    Points = myPointCollection,
                    Fill = Brushes.Blue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 5,
                    Opacity = 0.0,
                };
                Options.Disk.Children.Add(polygon);
                polygon.MouseEnter += Polygon_MouseEnter;
                polygon.Name = $"Polygon_{i}";
            }
        }
        private void Polygon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Polygon polygon = sender as Polygon;
            CurrentValue = System.Convert.ToInt32(polygon.Name.Substring(8));
        }

        private void Image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Options.canvas.MouseUp -= Image_MouseUp;
            while (Options.Disk.Children.Count > 0)
                Options.Disk.Children.RemoveAt(0);
        }

        protected void SetStartValue(int value, int maxValue)
        {
            this.maxValue = maxValue;
            currentValue = (value) % maxValue;
            Angle = currentValue;
        }
        protected int CurrentValue
        {
            get => currentValue;
            set
            {
                if (currentValue != value)
                {
                    if (maxAngle != 360)
                    {
                        if (value >= maxValue) currentValue = maxValue - 1;
                        else if (value < 0) currentValue = 0;
                        else currentValue = value;
                    }
                    else
                        currentValue = (value + maxValue) % maxValue;

                    Angle = currentValue;
                    PlaySound();
                    ValueIsUpdated();
                }
            }
        }
        protected new int Angle
        {
            get => System.Convert.ToInt32((base.Angle - minAngle) / maxAngle) * maxValue;
            set => base.Angle = System.Convert.ToDouble(value) / maxValue * (maxAngle - minAngle) + minAngle;
        }

        private void PlaySound()
        {
            try
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                player.SoundLocation = @"C:\Users\DK\Documents\R-123\R-123\R-123\sounds\PositionSwitcher.wav";
                player.Load();
                player.Play();
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine("Ошибка воспроизведения звукового файла");
            }
        }
    }
}
