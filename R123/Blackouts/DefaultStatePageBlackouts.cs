using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Drawing.Image;

namespace R123.Blackouts
{
    internal class DefaultStatePageBlackouts : TemplateBlackouts
    {
        private const int NUMBER_CIRCLES = 10;

        private static readonly int[][] data = new int[NUMBER_CIRCLES][]
        {
            new int[3] {211, 78, 86}, // 1
            new int[3] {115, 64, 71}, // 2
            new int[3] {75, 169, 77}, // 3
            new int[3] {42, 244, 35}, // 4
            new int[3] {107, 244, 35}, // 5
            new int[3] {759, 315, 73}, // 6
            new int[3] {585, 318, 125}, // 7
            new int[3] {369, 217, 124}, // 8
            new int[3] {759, 97, 84}, // 9
            new int[3] {569, 149, 42} // 10
        };

        private readonly string[] path;

        private readonly string[] text;

        public DefaultStatePageBlackouts(Path Background_Path, Canvas element) :
            base(Background_Path, element, data, 964, 450)
        {
            text = new[]
            {
                "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\"",
                "Ручку \"ШУМЫ\" поверните против часовой стрелки до упора, т.е. установите максимальные шумы приемника",
                "Переключатель \"КОНТРОЛЬ НАПРЯЖЕНИЙ\" установите в положение \"РАБОТА-1\"",
                "Тумблер \"ШКАЛА\" поставьте в положение \"ВКЛ\"",
                "Тумблер \"ПИТАНИЕ\" поставьте в положение \"ВКЛ\"",
                "Ручку регулятора \"ГРОМКОСТЬ\" поверните вправо до упора, т.е. установите максимальную громкость",
                "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в одно из положений \"I\", \"II\", \"II\" или \"IV\"",
                "Установите фиксаторы дисков в положение, параллельное линии круга",
                "Установите тумблеры \"ПОДДИАПАЗОН\" в положение 2",
                "Установите ручку \"НАСТРОЙКА АНТЕННЫ\" в крайнее правое положение"
            };

            path = new[]
            {
                @"../../Files/Gifs\simplex.gif",
                @"../../Files/Gifs\noiseToLeft.gif",
                @"../../Files/Gifs\broadcast1.gif",
                @"../../Files/Gifs\scaleOff.gif",
                @"../../Files/Gifs\powerOff.gif",
                @"../../Files/Gifs\volumeToRight.gif",
                @"../../Files/Gifs/GifsStep1\8.rangeToFixFrequency1.gif",
                @"../../Files/Gifs\fixClamp.gif",
                @"../../Files/Gifs\allSubFrequency2.gif",
                @"../../Files/Gifs\fixFixerFrequency.gif"
            };
        }

        public void SetPanels(StackPanel left, StackPanel right)
        {
            panels = new StackPanel[text.Length];
            for (var i = 0; i < panels.Length; i++)
                panels[i] = i < 5 || i == 7 ? right : left;

            elements = new StackPanel[text.Length];

            for (var i = 0; i < elements.Length; i++)
            {
                var panel = new StackPanel
                {
                    Background = Brushes.White,
                    Margin = new Thickness(5)
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
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                if (path != null && i < path.Length && !string.IsNullOrEmpty(path[i]))
                {
                    Image tempImage = new Bitmap(path[i]);

                    panel.Children.Add(new WindowsFormsHost
                    {
                        Child = new PictureBox
                        {
                            ImageLocation = path[i],
                            Height = tempImage.Height,
                            Width = tempImage.Width
                        },
                        Margin = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                }
            }
        }
    }
}