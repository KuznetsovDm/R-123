using System;
using System.Windows.Controls;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Work.xaml
    /// </summary>
    public partial class Work : Page
    {
        public Work()
        {
            InitializeComponent();

            IsVisibleChanged += (s, e) => Logic.PageChanged(Convert.ToBoolean(e.NewValue), Radio.Model);

            Radio.Model.MainModel Model = Radio.Model;

            Model.Frequency.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("NewFrequency: " + e.NewValue);
            Model.Frequency.EndValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("EndNewFrequency: " + e.NewValue);

            //Model.Clamps[0].ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Clamp0) = " + e.NewValue);
            //Model.Clamps[1].ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Clamp1) = " + e.NewValue);
            //Model.Clamps[2].ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Clamp2) = " + e.NewValue);
            //Model.Clamps[3].ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Clamp3) = " + e.NewValue);

            Model.Clamps.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Clamp" + e.Number + ") = " + e.NewValue);
        }
    }
}
