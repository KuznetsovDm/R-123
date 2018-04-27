using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace R123.Blackouts
{
    class ManagementBlackouts : TemplateBlackouts
    {
        private const int NUMBER_CIRCLES = 28;
        private static int[][] data = new int[NUMBER_CIRCLES][]
        {
            new int[]{ 408, 870, 91 },  // 1
            new int[]{ 254, 869, 110 }, // 2
            new int[]{ 419, 753, 90 },  // 3
            new int[]{ 309, 696, 46 },  // 4
            new int[]{ 230, 696, 46 },  // 5
            new int[]{ 268, 696, 46 },  // 6
            new int[]{ 270, 599, 111 }, // 7
            new int[]{ 250, 456, 86 },  // 8
            new int[]{ 318, 467, 86 },  // 9
            new int[]{ 418, 638, 96 },  // 10
            new int[]{ 437, 483, 121 }, // 11
            new int[]{ 547, 473, 81 },  // 12
            new int[]{ 633, 439, 131 }, // 13
            new int[]{ 763, 229, 121 }, // 14
            new int[]{ 773, 359, 91 },  // 15
            new int[]{ 823, 459, 71 },  // 16
            new int[]{ 930, 236, 121 }, // 17
            new int[]{ 878, 572, 131 }, // 18
            new int[]{ 877, 574, 61 },  // 19
            new int[]{ 1110, 429, 91 }, // 20
            new int[]{ 1110, 516, 91 }, // 21
            new int[]{ 1111, 618, 91 }, // 22
            new int[]{ 1283, 633, 91 }, // 23
            new int[]{ 1313, 713, 71 }, // 24
            new int[]{ 1112, 693, 91 }, // 25
            new int[]{ 1112, 782, 86 }, // 26
            new int[]{ 898, 786, 158 }, // 27
            new int[]{ 633, 661, 168 }, // 28
        };

        public ManagementBlackouts(Path Background_Path, Canvas element) : base(Background_Path, element, data, 1652, 1124)
        {
        }

        public void SetPanels(StackPanel left, StackPanel right)
        {
            string[] textForBlock = System.IO.File.ReadAllLines("../../StartTab/TextForManagement.txt");

            panels = new StackPanel[textForBlock.Length];
            for (int i = 0; i < panels.Length; i++)
                panels[i] = (i < 14 || i == 27) ? right : left;

            TextBlock[] blocks = new TextBlock[textForBlock.Length];
            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = new TextBlock()
                {
                    Text = textForBlock[i],
                    Margin = new Thickness(20),
                    Padding = new Thickness(20),
                    FontSize = 40,
                    FontFamily = new FontFamily("Times New Roman"),
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = Brushes.White,
                    MaxWidth = 600,
                };
            elements = blocks;
        }
    }
}
