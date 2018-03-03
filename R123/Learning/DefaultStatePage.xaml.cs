using System;
using System.Windows;
using System.Windows.Controls;
using R123.NewRadio.Model;

namespace R123.Learning
{
    /// <summary>
    /// Логика взаимодействия для DefaultStatePage.xaml
    /// </summary>
    public partial class DefaultStatePage : Page
    {
        //public Radio.View.RadioPage RadioPage { get; private set; }
        private Func<bool>[] Conditions { get; set; }
        private string[] Checks = {
            "Фиксатор ручки \"НАСТРОЙКА АНТЕННЫ\" затянут",
            "Фиксаторы дисков установки частоты затянуты",
            "Переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в одном из положений \"ФИКСИР, ЧАСТОТЫ 1, 2, 3 или 4\"",
            "Регулятор \"ГРОМКОСТЬ\" выведен на максимум громкости",
            "Регулятор \"ШУМЫ\" выведен (в левом крайнем положении)",
            "Переключатель контроля напряжений в положении \"РАБОТА 1\"",
            "Переключатель рода работ в положении \"СИМПЛЕКС\"",
            "Тумблеры \"ПОДДИАПАЗОН\" каждый в положении, соответствующем заданной фиксированной частоте",
            "Тумблер \"ШКАЛА\" в положении \"ВЫКЛ\"",
            "Тумблер \"ПИТАНИЕ\" в положении \"ВЫКЛ\""
        };

        public DefaultStatePage()
        {
            InitializeComponent();
            InitializeControls();

            Conditions = new Func<bool>[10];
            Conditions[0] = () => Radio.Model.AntennaFixer.Value == ClampState.Fixed;
            Conditions[1] = () => Radio.Model.Clamps[0] == ClampState.Fixed &&
                                  Radio.Model.Clamps[1] == ClampState.Fixed &&
                                  Radio.Model.Clamps[2] == ClampState.Fixed &&
                                  Radio.Model.Clamps[3] == ClampState.Fixed;
            Conditions[2] = () => Radio.Model.Range.Value >= 0 && (int)Radio.Model.Range.Value < 4;
            Conditions[3] = () => Radio.Model.Volume.Value == 1.0;
            Conditions[4] = () => Radio.Model.Noise.Value == 1.0;
            Conditions[5] = () => Radio.Model.Voltage.Value == VoltageState.Broadcast1;
            Conditions[6] = () => Radio.Model.WorkMode.Value == WorkModeState.Simplex;
            Conditions[7] = () => true;
            Conditions[8] = () => Radio.Model.Scale.Value == Turned.Off;
            Conditions[9] = () => Radio.Model.Power.Value == Turned.Off;

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
                if (CheckCondition(i)) {
                    ((CheckBox)((StackPanel)panel.Children[i]).Children[0]).IsChecked = true;
                }
                else {
                    ((CheckBox)((StackPanel)panel.Children[i]).Children[0]).IsChecked = false;
                    allChecked = false;
                }
            }

            if (allChecked) {
                Message message = new Message("Все органы управления находятся в исходном положении.", false);
                message.ShowDialog();
                InitializeUnsubscribe();
                MainWindow.Instance.ActivateTab(2);
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
            Radio.Model.Clamps.ValueChanged += Check;
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
            Radio.Model.Clamps.ValueChanged -= Check;
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
        }
    }
}
