using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using R123.Files;
using R123.Radio.Model;

namespace R123.Learning
{
    /// <summary>
    /// Логика взаимодействия для DefaultStatePage.xaml
    /// </summary>
    public partial class DefaultStatePage : Page, IRestartable
    {
        private SolidColorBrush red = new SolidColorBrush(Colors.Red);
        private SolidColorBrush green = new SolidColorBrush(Colors.Green);

        private Func<bool>[] Conditions { get; set; }
        private string[] Checks = {
            "Фиксатор ручки \"НАСТРОЙКА АНТЕННЫ\" затянут",
            "Фиксаторы дисков установки частоты затянуты",
            "Переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в одном из положений \"ФИКСИР, ЧАСТОТЫ 1, 2, 3 или 4\"",
            "Регулятор \"ГРОМКОСТЬ\" выведен на максимум громкости",
            "Регулятор \"ШУМЫ\" выведен (в левом крайнем положении)",
            "Переключатель контроля напряжений в положении \"РАБОТА 1\"",
            "Переключатель рода работ в положении \"СИМПЛЕКС\"",
            "Тумблеры \"ПОДДИАПАЗОН\" каждый в положении \"ПОДДИАПАЗОН II\"",
            "Тумблер \"ШКАЛА\" в положении \"ВЫКЛ\"",
            "Тумблер \"ПИТАНИЕ\" в положении \"ВЫКЛ\""
        };

        public DefaultStatePage()
        {
            InitializeComponent();

            InitializeControls();

            Conditions = new Func<bool>[10];
            Conditions[0] = () => Radio.Model.AntennaFixer.Value == ClampState.Fixed;
            Conditions[1] = () => Radio.Model.Clamps[0].Value == ClampState.Fixed &&
                                  Radio.Model.Clamps[1].Value == ClampState.Fixed &&
                                  Radio.Model.Clamps[2].Value == ClampState.Fixed &&
                                  Radio.Model.Clamps[3].Value == ClampState.Fixed;
            Conditions[2] = () => Radio.Model.Range.Value >= 0 && (int)Radio.Model.Range.Value < 4;
            Conditions[3] = () => Radio.Model.Volume.Value == 1.0;
            Conditions[4] = () => Radio.Model.Noise.Value == 1.0;
            Conditions[5] = () => Radio.Model.Voltage.Value == VoltageState.Broadcast1;
            Conditions[6] = () => Radio.Model.WorkMode.Value == WorkModeState.Simplex;
            Conditions[7] = () => Radio.Model.SubFixFrequency[0].Value == Turned.Off &&
                                  Radio.Model.SubFixFrequency[1].Value == Turned.Off &&
                                  Radio.Model.SubFixFrequency[2].Value == Turned.Off &&
                                  Radio.Model.SubFixFrequency[3].Value == Turned.Off;
            Conditions[8] = () => Radio.Model.Scale.Value == Turned.Off;
            Conditions[9] = () => Radio.Model.Power.Value == Turned.Off;

            foreach (string check in Checks) {
                StackPanel horizontalPanel = new StackPanel {
                    Orientation = Orientation.Horizontal
                };

                horizontalPanel.Children.Add(new NewCheckBox {
                    IsChecked = false,
                    Width = 18,
                    Height = 18
                });

                horizontalPanel.Children.Add(
                    new TextBlock {
                        Text = check,
                        MaxWidth = 900,
                        Foreground = new SolidColorBrush(Colors.Red)
                });

                panel.Children.Add(horizontalPanel);
            }

            InitializeSubscribe();
        }

        private void Check(object sender, EventArgs args)
        {
            bool allChecked = true;
            for (int i = 0; i < Conditions.Length; i++) {
                if (CheckCondition(i)) {
                    NewCheckBox checkBox = (NewCheckBox)((StackPanel)panel.Children[i]).Children[0];
                    checkBox.IsChecked = true;
                    TextBlock textBlock = (TextBlock)((StackPanel)panel.Children[i]).Children[1];
                    textBlock.Foreground = green;
                }
                else {
                    NewCheckBox checkBox = (NewCheckBox)((StackPanel)panel.Children[i]).Children[0];
                    checkBox.IsChecked = false;
                    TextBlock textBlock = (TextBlock)((StackPanel)panel.Children[i]).Children[1];
                    textBlock.Foreground = red;
                    allChecked = false;
                }
            }

            if (allChecked) {
                Message message = new Message("Все органы управления находятся в исходном положении.", false);
                message.ShowDialog();
                InitializeUnsubscribe();
                MainScreens.WorkOnRadioStation.Instance.ActivateStep(1);
            }
        }

        private bool CheckCondition(int index)
        {
            return Conditions[index]();
        }

        private void InitializeSubscribe()
        {
            Radio.Model.Noise.ValueChanged += Check;
            Radio.Model.Voltage.ValueChanged += Check;
            Radio.Model.Power.ValueChanged += Check;
            Radio.Model.Scale.ValueChanged += Check;
            Radio.Model.WorkMode.ValueChanged += Check;
            Radio.Model.Volume.ValueChanged += Check;
            Radio.Model.Range.ValueChanged += Check;
            for (int i = 0; i < Radio.Model.Clamps.Length; i++)
                Radio.Model.Clamps[i].ValueChanged += Check;
            for (int i = 0; i < Radio.Model.SubFixFrequency.Length; i++)
                Radio.Model.SubFixFrequency[i].ValueChanged += Check;
            Radio.Model.NumberSubFrequency.ValueChanged += Check;
            Radio.Model.AntennaFixer.ValueChanged += Check;
        }

        private void InitializeUnsubscribe()
        {
            Radio.Model.Noise.ValueChanged -= Check;
            Radio.Model.Voltage.ValueChanged -= Check;
            Radio.Model.Power.ValueChanged -= Check;
            Radio.Model.Scale.ValueChanged -= Check;
            Radio.Model.WorkMode.ValueChanged -= Check;
            Radio.Model.Volume.ValueChanged -= Check;
            Radio.Model.Range.ValueChanged -= Check;
            for (int i = 0; i < Radio.Model.Clamps.Length; i++)
                Radio.Model.Clamps[i].ValueChanged -= Check;
            for (int i = 0; i < Radio.Model.SubFixFrequency.Length; i++)
                Radio.Model.SubFixFrequency[i].ValueChanged -= Check;
            Radio.Model.NumberSubFrequency.ValueChanged -= Check;
            Radio.Model.AntennaFixer.ValueChanged -= Check;
        }

        private void InitializeControls()
        {
            Radio.Model.Noise.Value = 0.5;
            Radio.Model.Voltage.Value = VoltageState.Broadcast250;
            Radio.Model.Power.Value = Turned.On;
            Radio.Model.Scale.Value = Turned.On;
            Radio.Model.WorkMode.Value = WorkModeState.WasIstDas;
            Radio.Model.Volume.Value = 0.5;
            Radio.Model.Range.Value = RangeState.SmoothRange2;
            Radio.Model.AntennaFixer.Value = ClampState.Medium;
            Radio.Model.Clamps[0].Value = ClampState.Medium;
            Radio.Model.Clamps[1].Value = ClampState.Medium;
            Radio.Model.Clamps[2].Value = ClampState.Medium;
            Radio.Model.Clamps[3].Value = ClampState.Medium;
            Radio.Model.SubFixFrequency[0].Value = Turned.On;
            Radio.Model.SubFixFrequency[1].Value = Turned.On;
            Radio.Model.SubFixFrequency[2].Value = Turned.On;
            Radio.Model.SubFixFrequency[3].Value = Turned.On;
        }

        public void Restart()
        {
            for (int i = 0; i < Conditions.Length; i++)
            {
                NewCheckBox checkBox = (NewCheckBox)((StackPanel)panel.Children[i]).Children[0];
                checkBox.IsChecked = false;
                ((TextBlock)((StackPanel)panel.Children[i]).Children[1]).Foreground = red;
            }

            InitializeControls();
            InitializeUnsubscribe();
            InitializeSubscribe();
        }
    }
}
