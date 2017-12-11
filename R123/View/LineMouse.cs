using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace R123.View
{
    public class LineMouse
    {
        Line line;
        public LineMouse(Line line)
        {
            this.line = line;
        }
        public void Visibility(bool visible)
        {
            if (visible)
                line.Visibility = System.Windows.Visibility.Visible;
            else
                line.Visibility = System.Windows.Visibility.Hidden;
        }
        public void SetCenter(double x1, double y1)
        {
            line.X1 = x1;
            line.Y1 = y1;
        }
        public void SetMouse(double x2, double y2)
        {
            line.X2 = x2;
            line.Y2 = y2;
        }
    }
}
