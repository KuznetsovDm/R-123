using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace R_123.View
{
    class PositionSwitcher : ImagesControl
    {
        Audio.AudioPlayer player = new Audio.AudioPlayer("../../Files/Sounds/PositionSwitcher.wav");
        private int maxValue = 20;
        private int currentValue = 0;
        private double defAngle = 0;
        protected double minAngle = 0;
        protected double maxAngle = 360;
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
            CurrentValue += e.Delta > 0 ? 1 : -1;
        }
        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Options.Window.MouseUp += Image_MouseUp;
            double centerX = Canvas.GetLeft(Image) + Image.ActualWidth / 2;
            double centerY = Canvas.GetTop(Image) + Image.ActualHeight / 2;
            double startAngle = minAngle * System.Math.PI * 2 / 360;
            double stepAngle = System.Math.PI * 2 * (maxAngle - minAngle) / 360 / maxValue;
            for (int i = 0; i < maxValue; i++)
            {
                double point1 = (i - 0.5 + defAngle) * stepAngle - startAngle;
                double point2 = (i + 0.5 + defAngle) * stepAngle - startAngle;
                double point1x = System.Math.Cos(point1) * 1500;
                double point1y = System.Math.Sin(point1) * 1500;
                double point2x = System.Math.Cos(point2) * 1500;
                double point2y = System.Math.Sin(point2) * 1500;

                PointCollection myPointCollection = new PointCollection
                {
                    new Point(centerX, centerY),
                    new Point(centerX+point1x, centerY+point1y),
                    new Point(centerX+point2x, centerY+point2y)
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
            Options.Window.MouseUp -= Image_MouseUp;
            while (Options.Disk.Children.Count > 0)
                Options.Disk.Children.RemoveAt(0);
        }
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Options.Window.MouseUp += Window_MouseUp;
            Options.Window.MouseMove += Window_MouseMove;
        }
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point point = e.GetPosition(Options.R123 as IInputElement);
            Vector norm = new Vector(1.0, 0.0);
            Vector mouse = new Vector(point.X - Canvas.GetLeft(Image), point.Y - Canvas.GetTop(Image));
            double angle = Vector.AngleBetween(norm, mouse);
            System.Diagnostics.Trace.WriteLine(angle);
            //CurrentValue = System.Convert.ToInt32(System.Math.Round(angle / 360 * maxValue));
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Options.Window.MouseUp -= Window_MouseUp;
            Options.Window.MouseMove -= Window_MouseMove;
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
                    //player.Start();
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
                System.Media.SoundPlayer player = new System.Media.SoundPlayer
                {
                    SoundLocation = @"D:\project\R-123\R-123\Files\Sounds\PositionSwitcher.wav"
                };
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
