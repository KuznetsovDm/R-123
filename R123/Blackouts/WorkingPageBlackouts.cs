using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Drawing.Image;
using Path = System.Windows.Shapes.Path;

namespace R123.Blackouts
{
    internal class WorkingPageBlackouts : TemplateBlackouts
    {
        private const int NUMBER_CIRCLES = 18;

        private static readonly int[][] data = new int[NUMBER_CIRCLES][]
        {
            new int[3] {212, 79, 79},
            new int[3] {115, 64, 42},
            new int[3] {75, 244, 66},
            new int[3] {760, 184, 64},
            new int[3] {760, 314, 64},
            new int[3] {585, 323, 116},
            new int[3] {197, 204, 62},
            new int[3] {115, 64, 42},
            new int[3] {212, 79, 79},
            new int[3] {76, 245, 30},
            new int[3] {212, 79, 79},
            new int[3] {908, 118, 84},
            new int[3] {658, 118, 181},
            new int[3] {76, 245, 30},
            new int[3] {368, 218, 82},
            new int[3] {658, 118, 181},
            new int[3] {584, 320, 121},
            new int[3] {107, 246, 26}
        };

        private readonly string[] path;

        private readonly string[] text;

        public WorkingPageBlackouts(Path backgroundPath, Canvas element) : base(backgroundPath, element, data, 964,
            450)
        {

            text = new[]
            {
                "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\"",
                "Ручку \"ШУМЫ\" поверните против часовой стрелки до упора, т.е. установите максимальные шумы приемника",
                "Тумблеры \"ПИТАНИЕ\" и \"ШКАЛА\" установите в положение \"ВКЛ\"",
                "Зажмите пробел и убедитесь, что стрелка вольтметра отклонилась от нулевого положения",
                "Ручку регулятора \"ГРОМКОСТЬ\" поверните вправо до упора, т.е. установите максимальную громкость",
                "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН I\"",
                "Повращайте ручку установки частоты",
                "Повращайте ручку \"ШУМЫ\" от максимального до минимального значения. Вы должны услышать уменьшения громкости",
                "Переключатель рода работ поставьте в положение \"ДЕЖ. ПРИЕМ\"",
                "Нажмите кнопку \"ТОН-ВЫЗОВ\"",
                "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\"",
                "Зажмите пробел и выполняйте следующий пункт",
                "Зажав пробел, вращайте ручку \"НАСТРОЙКА АНТЕННЫ\", пока стрелка индикатора не отклонится в максимально правое положение",
                "Нажмите кнопку \"ТОН-ВЫЗОВ\"",
                "Зафиксируйте фиксаторы, установив их параллельно линии круга",
                "Зажав пробел, вращайте ручку \"НАСТРОЙКА АНТЕННЫ\", пока стрелка индикатора не отклонится в максимально правое положение",
                "Последовательно установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ФИКС. ЧАСТОТА 1\", \"ФИКС. ЧАСТОТА 2\", \"ФИКС. ЧАСТОТА 3\" и\"ФИКС. ЧАСТОТА 4\"",
                "Тумблер \"ПИТАНИЕ\" установите в положение \"ВЫКЛ\""
            };

            path = new[]
            {
                @"../../Files/Gifs\simplex.gif",
                @"../../Files/Gifs\noiseToLeft.gif",
                @"../../Files/Gifs\powerAndScaleOn.gif",
                @"../../Files/Gifs\testVoltagePower.gif",
                @"../../Files/Gifs\volumeToRight.gif",
                @"../../Files/Gifs\rangeSubFrequency1.gif",
                @"../../Files/Gifs\listenFrequency.gif",
                @"../../Files/Gifs\listenNoise.gif",
                @"../../Files/Gifs\setStandbyReception.gif",
                @"../../Files/Gifs\testTone.gif",
                @"../../Files/Gifs\simplex.gif",
                @"../../Files/Gifs\prd.gif",
                @"../../Files/Gifs\setSettingsAntenna.gif",
                @"../../Files/Gifs\testTone.gif",
                @"../../Files/Gifs\fixClamp.gif",
                @"../../Files/Gifs\setSettingsAntenna.gif",
                @"../../Files/Gifs\testFixFrequency.gif",
                @"../../Files/Gifs\powerOff.gif"
            };


            drawer.Write = coord => { WriteInFile("coordinates", "working", "new int[3] { " + coord + " },\r\n"); };
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
                panels[i] = i < 3 || 5 < i && i < 11 || i == 13 || i == 14 || i == 17 ? right : left;

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

                if (path == null || i >= path.Length || string.IsNullOrEmpty(path[i])) continue;
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