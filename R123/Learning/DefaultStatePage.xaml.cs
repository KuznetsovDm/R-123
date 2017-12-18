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

namespace R123.Learning
{
    /// <summary>
    /// Логика взаимодействия для DefaultStatePage.xaml
    /// </summary>
    public partial class DefaultStatePage : Page
    {
        public Radio.View.RadioPage RadioPage { get; private set; }
        private Func<bool>[] Conditions { get; set; }
        private string[] Checks = {
            "Фиксатор ручки \"НАСТРОЙКА АНТЕННЫ\" затянут",
            "Фиксаторы дисков установки частоты затянуты",
            "Переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в одном из положений \"ФИКСИР, ЧАСТОТЫ 1, 2, 3 или 4\"",
            "Регулятор \"ГРОМКОСТЬ\" выведен на максимум громкости",
            "Регулятор \"ШУМЫ\" выведен (в левом крайнем положении)",
            "Переключатель контроля напряжений в положении \"РАБОТА \"",
            "Переключатель рода работ в положении \"СИМПЛЕКС\"",
            "Тумблеры \"ПОДДИАПАЗОН\" каждый в положении, соответствующем заданной фиксированной частоте",
            "Тумблер \"ШКАЛА\" в положении \"ВЫКЛ\"",
            "Тумблер \"ПИТАНИЕ\" в положении \"ВЫКЛ\""
        };

        public DefaultStatePage()
        {
            InitializeComponent();
            RadioPage = new Radio.View.RadioPage();
            frame.Content = RadioPage;

            Conditions = new Func<bool>[10];
            Conditions[0] = () => RadioPage.Radio.AntennaClip.Value == 1.0;
            Conditions[1] = () => !(RadioPage.Radio.Clamp[0].Value || RadioPage.Radio.Clamp[1].Value
                                || RadioPage.Radio.Clamp[2].Value || RadioPage.Radio.Clamp[3].Value);
            Conditions[2] = () => RadioPage.Radio.Range.Value >= 0 && RadioPage.Radio.Range.Value < 4;
            Conditions[3] = () => RadioPage.Radio.Volume.Value == 1.0;
            Conditions[4] = () => RadioPage.Radio.Noise.Value == 1.0;
            Conditions[5] = () => RadioPage.Radio.Voltage.Value == 0;
            Conditions[6] = () => RadioPage.Radio.WorkMode.Value == 1;
            Conditions[7] = () => true;
            Conditions[8] = () => !RadioPage.Radio.Scale.Value;
            Conditions[9] = () => !RadioPage.Radio.Power.Value;

            foreach (string check in Checks) {
                StackPanel horizontalPanel = new StackPanel {
                    Orientation = Orientation.Horizontal
                };

                horizontalPanel.Children.Add(new CheckBox {
                    VerticalAlignment = VerticalAlignment.Center
                });
                horizontalPanel.Children.Add(
                    new TextBlock {
                        Text = check,
                        MaxWidth = 900
                });

                panel.Children.Add(horizontalPanel);
            }

            InitializeSubscribe();
        }

        private void Check(object sender, EventArgs args)
        {
            bool allChecked = true;
            for (int i = 0; i < Conditions.Length; i++) {
                if (Conditions[i]()) {
                    ((CheckBox)((StackPanel)panel.Children[i]).Children[0]).IsChecked = true;
                }
                else allChecked = false;
            }

            if (allChecked) {
                Message message = new Message("Все органы управления находятся в исходном положении", false);
                message.ShowDialog();
                InitializeUnsubscribe();
            }
        }

        private void InitializeSubscribe()
        {
            RadioPage.Radio.Noise.ValueChanged += Check;
            RadioPage.Radio.Voltage.ValueChanged += Check;
            RadioPage.Radio.Power.ValueChanged += Check;
            RadioPage.Radio.Scale.ValueChanged += Check;
            RadioPage.Radio.WorkMode.ValueChanged += Check;
            RadioPage.Radio.Volume.ValueChanged += Check;
            RadioPage.Radio.Range.ValueChanged += Check;
            RadioPage.Radio.Clamp[0].ValueChanged += Check;
            RadioPage.Radio.Clamp[1].ValueChanged += Check;
            RadioPage.Radio.Clamp[2].ValueChanged += Check;
            RadioPage.Radio.Clamp[3].ValueChanged += Check;
            RadioPage.Radio.SubFixFrequency[0].ValueChanged += Check;
            RadioPage.Radio.SubFixFrequency[1].ValueChanged += Check;
            RadioPage.Radio.SubFixFrequency[2].ValueChanged += Check;
            RadioPage.Radio.SubFixFrequency[3].ValueChanged += Check;
            RadioPage.Radio.AntennaClip.ValueChanged += Check;
        }

        private void InitializeUnsubscribe()
        {
            RadioPage.Radio.Noise.ValueChanged -= Check;
            RadioPage.Radio.Voltage.ValueChanged -= Check;
            RadioPage.Radio.Power.ValueChanged -= Check;
            RadioPage.Radio.Scale.ValueChanged -= Check;
            RadioPage.Radio.WorkMode.ValueChanged -= Check;
            RadioPage.Radio.Volume.ValueChanged -= Check;
            RadioPage.Radio.Range.ValueChanged -= Check;
            RadioPage.Radio.Clamp[0].ValueChanged -= Check;
            RadioPage.Radio.Clamp[1].ValueChanged -= Check;
            RadioPage.Radio.Clamp[2].ValueChanged -= Check;
            RadioPage.Radio.Clamp[3].ValueChanged -= Check;
            RadioPage.Radio.SubFixFrequency[0].ValueChanged -= Check;
            RadioPage.Radio.SubFixFrequency[1].ValueChanged -= Check;
            RadioPage.Radio.SubFixFrequency[2].ValueChanged -= Check;
            RadioPage.Radio.SubFixFrequency[3].ValueChanged -= Check;
            RadioPage.Radio.AntennaClip.ValueChanged -= Check;
        }
    }
}
