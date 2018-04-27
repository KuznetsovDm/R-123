using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Shapes;

namespace R123.Blackouts
{
    class DefaultStatePageBlackouts : TemplateBlackouts
    {
        private const int NUMBER_CIRCLES = 10;
        private static int[][] data = new int[NUMBER_CIRCLES][]
        {
            new int[3]{ 211, 78, 86 },   // 1
            new int[3]{ 115, 64, 71 },   // 2
            new int[3]{ 75, 169, 77 },   // 3
            new int[3]{ 42, 244, 35 },   // 4
            new int[3]{ 107, 244, 35 },  // 5
            new int[3]{ 759, 315, 73 },  // 6
            new int[3]{ 585, 318, 125 }, // 7
            new int[3]{ 369, 217, 124 }, // 8
            new int[3]{ 759, 97, 84 },   // 9
            new int[3]{ 569, 149, 42 },  // 10
        };

        private string[] text;
        private string[] path;
        
        public DefaultStatePageBlackouts(Path Background_Path, Canvas element, string[] text, string[] path) : 
            base(Background_Path, element, data, 964, 450)
        {
            this.text = text;
            this.path = path;
        }

        public void SetPanels(StackPanel left, StackPanel right)
        {
            panels = new StackPanel[text.Length];
            for (int i = 0; i < panels.Length; i++)
                panels[i] = i < 5 || i == 7 ? right : left;

            elements = new StackPanel[text.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                StackPanel panel = new StackPanel()
                {
                    Background = Brushes.White,
                    Margin = new Thickness(5),
                };
                elements[i] = panel;

                panel.Children.Add(new TextBlock
                {
                    Text = text[i],
                    Padding = new Thickness(0),
                    Foreground = Brushes.Black,
                    FontSize = 20,
                    FontFamily = new FontFamily("Times New Roman"),
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                });

                if (path != null && i < path.Length && !string.IsNullOrEmpty(path[i]))
                {
                    System.Drawing.Image tempImage = new System.Drawing.Bitmap(path[i]);

                    panel.Children.Add(new WindowsFormsHost
                    {
                        Child = new System.Windows.Forms.PictureBox()
                        {
                            ImageLocation = path[i],
                            Height = tempImage.Height,
                            Width = tempImage.Width
                        },
                        Margin = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Center,
                    });
                }
            }
        }
    }
}
