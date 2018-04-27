using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace R123.Blackouts
{
    abstract class TemplateBlackouts
    {
        protected readonly Drawer drawer;
        private readonly Canvas canvas;
        private StackPanel lastPanel = new StackPanel();
        protected StackPanel[] panels = new StackPanel[0];
        protected UIElement[] elements = new UIElement[0];

        public TemplateBlackouts(Path Background_Path, Canvas canvas, int[][] data, int width, int height)
        {
            drawer = new Drawer(Background_Path, width, height, false);
            drawer.SetCircles(data);
            this.canvas = canvas;

            foreach (UIElement element in canvas.Children)
            {
                element.MouseEnter += Border_MouseEnter;
                element.MouseLeave += Border_MouseLeave;
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            int number = canvas.Children.IndexOf(sender as UIElement);
            drawer.ShowBlackout(number);

            if (number < panels.Length && number < elements.Length)
            {
                panels[number].Children.Add(elements[number]);
                panels[number].Visibility = Visibility.Visible;
                lastPanel = panels[number];
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            drawer.DeleteBlackout();

            while (lastPanel.Children.Count > 0)
                lastPanel.Children.RemoveAt(0);
            lastPanel.Visibility = Visibility.Hidden;
        }
    }
}
