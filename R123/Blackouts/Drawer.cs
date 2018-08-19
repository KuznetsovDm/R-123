using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace R123.Blackouts
{
    class Drawer
    {
        private readonly Path path;
        private readonly RectangleGeometry background;
        private Point centerCircle = new Point();
        private EllipseGeometry circle;

        public Action<string> Write { get; set; }

        public Drawer(Path path, int width, int height, bool addControlledCircle = false)
        {
            this.path = path;
            background = new RectangleGeometry(new Rect(0, 0, width, height));
            path.Fill = new SolidColorBrush(Color.FromScRgb(0.8f, 0, 0, 0));

            if (addControlledCircle)
            {
                centerCircle.X = 100; centerCircle.Y = 100;
                circle = new EllipseGeometry(centerCircle, 50, 50);
                MainWindow.Instance.KeyDown += ControlCircle;
            }
        }

        EllipseGeometry[] ellipses;
        CombinedGeometry[] blackouts;

        public void SetCircles(int[][] data)
        {
            Point temp = new Point();
            ellipses = new EllipseGeometry[data.Length];
            blackouts = new CombinedGeometry[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                temp.X = data[i][0];
                temp.Y = data[i][1];
                ellipses[i] = new EllipseGeometry(temp, data[i][2], data[i][2]);
                blackouts[i] = new CombinedGeometry // background and (background xor ellipse)
                (
                    new CombinedGeometry // background xor ellipse
                    (
                        background,
                        ellipses[i]
                    )
                    {
                        GeometryCombineMode = GeometryCombineMode.Xor
                    },
                    background
                )
                {
                    GeometryCombineMode = GeometryCombineMode.Intersect
                };
            }
        }

        public void ShowBlackout(int number)
        {
            if (number >= ellipses.Length) return;

            path.Data = blackouts[number];
            circle = ellipses[number];
        }

        public void DeleteBlackout()
        {
            path.Data = null;
        }

        private void UpdateFigure()
        {
            path.Data = new CombinedGeometry // background and (background xor ellipse)
            (
                new CombinedGeometry // background xor ellipse
                (
                    background,
                    circle
                )
                {
                    GeometryCombineMode = GeometryCombineMode.Xor
                },
                background
            )
            {
                GeometryCombineMode = GeometryCombineMode.Intersect
            };
        }

        private void ControlCircle(object sender, KeyEventArgs e)
        {
            int step = 1;
            if (Keyboard.IsKeyDown(Key.LeftCtrl)) step = 10;

            if (Keyboard.IsKeyDown(Key.W)) centerCircle.Y -= step;
            if (Keyboard.IsKeyDown(Key.S)) centerCircle.Y += step;
            if (Keyboard.IsKeyDown(Key.A)) centerCircle.X -= step;
            if (Keyboard.IsKeyDown(Key.D)) centerCircle.X += step;
            if (Keyboard.IsKeyDown(Key.E)) circle.RadiusX = circle.RadiusY += step;
            if (Keyboard.IsKeyDown(Key.Q)) circle.RadiusX = circle.RadiusY -= step;
            if (Keyboard.IsKeyDown(Key.F)) WriteCoordinates($"{centerCircle.X}, {centerCircle.Y}, {circle.RadiusX}");

            circle.Center = centerCircle;

            UpdateFigure();
        }

        private void WriteCoordinates(string coordinates)
        {
            Write?.Invoke(coordinates);
        }
    }
}
