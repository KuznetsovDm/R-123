using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Drawing.Image;
using Path = System.Windows.Shapes.Path;

namespace R123.Blackouts
{
    internal class TuningPageBlackouts : TemplateBlackouts
    {
        private const int NUMBER_CIRCLES = 14;

        private static readonly int[][] data =
        {
            new[] {211, 79, 76},
            new[] {115, 64, 43},
            new[] {75, 172, 62},
            new[] {42, 246, 26},
            new[] {107, 246, 26},
            new[] {759, 316, 58},
            new[] {584, 322, 117},
            new[] {369, 219, 106},
            new[] {309, 229, 171},
            new[] {712, 99, 27},
            new[] {912, 119, 92},
            new[] {660, 100, 184},
            new[] {912, 119, 92},

            //new[] {211, 79, 76},
            //new[] {211, 79, 0}
        };

        private readonly string[] path =
        {
            @"../../Files/Gifs\simplex.gif",
            @"../../Files/Gifs\noiseToLeft.gif",
            @"../../Files/Gifs\broadcast1.gif",
            @"../../Files/Gifs\scaleOn.gif",
            @"../../Files/Gifs\powerOn.gif",
            @"../../Files/Gifs\volumeToRight.gif",
            @"../../Files/Gifs/GifsStep1\8.rangeToFixFrequency1.gif",
            @"../../Files/Gifs\clamp1Open.gif",
            @"../../Files/Gifs\workFrequencyAndClampClose.gif",
            @"../../Files/Gifs\subFrequency1.gif",
            @"../../Files/Gifs\prd.gif",
            @"../../Files/Gifs\setSettingsAntenna.gif",
            //@"../../Files/Gifs\setStandbyReception.gif"
        };

        private readonly string[] text =
        {
            "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\"",
            "Ручку \"ШУМЫ\" поверните против часовой стрелки до упора, т.е. установите максимальные шумы приемника",
            "Переключатель \"КОНТРОЛЬ НАПРЯЖЕНИЙ\" установите в положение \"РАБОТА-1\"",
            "Тумблер \"ШКАЛА\" поставьте в положение \"ВКЛ\"",
            "Тумблер \"ПИТАНИЕ\" поставьте в положение \"ВКЛ\"",
            "Ручку регулятора \"ГРОМКОСТЬ\" поверните вправо до упора, т.е. установите максимальную громкость",
            "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"1\"",
            "Расфиксируйте фиксатор-1, для этого поверните его перпендикулярно линии круга",
            "Установите рабочую частоту и зафиксируйте фиксатор-1, для этого поверните его параллельно линии круга",
            "Установите поддиапазон I",
            "Установите ПРД (нажмите пробел)",
			"Расфиксируйте фиксатор ручки настройки антенны. Зажав пробел, настройте антенну на максимальную мощность(вращайте до тех пор, пока стрелка индикатора не отклонится в максимально правое положение) и зафиксируйте фиксатор ручки настройки антенны",
            "Отпустите пробел",
            "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"4\""
        };

        public TuningPageBlackouts(Path Background_Path, Canvas element) : base(Background_Path, element, data, 964,
            450)
        {
            drawer.Write = coord => { WriteInFile("coordinates", "tuning", "new int[3] { " + coord + " },\r\n"); };
        }

        public static void WriteInFile(string directory, string name, string text,
            FileMode fileMode = FileMode.Append, FileAccess fileAccess = FileAccess.Write)
        {
            var dir = new DirectoryInfo(directory);
            if (!dir.Exists)
                dir.Create();
            var buff = Encoding.UTF8.GetBytes(text);
            using (var fileStream = new FileStream($"{directory}\\{name}.txt", fileMode, fileAccess))
            {
                fileStream.Write(buff, 0, buff.Length);
            }
        }

        public void SetPanels(StackPanel left, StackPanel right)
        {
            panels = new StackPanel[text.Length];
            
            for (var i = 0; i < panels.Length; i++)
                panels[i] = i < 5 || i == 7 || i == 8 || i == 12 ? right : left;

            //elements = new StackPanel[text.Length];
            elements = new Border[text.Length];

            for (var i = 0; i < elements.Length; i++)
            {
                var border = new Border
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    CornerRadius = new CornerRadius(5),
                    BorderThickness = new Thickness(3)
                };

                var panel = new StackPanel
                {
                    Background = Brushes.White
                };

                border.Child = panel;

                //elements[i] = panel;
                elements[i] = border;

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