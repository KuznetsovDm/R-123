using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace R123.Pages
{
    /// <summary>
    /// Логика взаимодействия для RadiostationPage.xaml
    /// </summary>
    public partial class RadiostationPage : Page
    {
        private readonly string scrollOrLeftButton = "крутите колесико мышки или зажмите ЛКМ и вращайте";

        public RadiostationPage()
        {
            InitializeComponent();

            volumeControl_Image.MouseEnter += VolumeControl_Image_MouseEnter;
            volumeControl_Image.MouseLeave += OnMouseLeave;

            voltageSwitcher_Image.MouseEnter += VoltageSwitcher_Image_MouseEnter;
            voltageSwitcher_Image.MouseLeave += OnMouseLeave;

            noiseControl_Image.MouseEnter += NoiseControl_Image_MouseEnter;
            noiseControl_Image.MouseLeave += OnMouseLeave;

            frequencyControl_Image.MouseEnter += FrequencyControl_Image_MouseEnter;
            frequencyControl_Image.MouseLeave += OnMouseLeave;
        }

        private void FrequencyControl_Image_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock.Text = $"Ручка \"УСТАНОВКА ЧАСТОТЫ\"{Environment.NewLine}Для регулировки {scrollOrLeftButton}";
        }

        private void NoiseControl_Image_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock.Text = $"Ручка регулятора шума \"ШУМЫ\"{Environment.NewLine}Для регулировки {scrollOrLeftButton}";
        }

        private void VoltageSwitcher_Image_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock.Text = $"Переключатель \"Контроль напряжений\"{Environment.NewLine}Для переключения {scrollOrLeftButton}";
        }
        
        private void VolumeControl_Image_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock.Text = $"Ручка регулятора громкости - \"ГРОМКОСТЬ\"{Environment.NewLine}Для регулировки {scrollOrLeftButton}";
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            textBlock.Text = "";
        }
    }
}
