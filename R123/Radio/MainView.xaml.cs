using System;
using System.Windows.Controls;

namespace R123.Radio
{
    /// <summary>
    /// Логика взаимодействия для Radio.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        private ViewModel.ViewModel ViewModel;
        public MainView()
        {
            InitializeComponent();

            DataContext = ViewModel = new ViewModel.ViewModel();

            IsVisibleChanged += (s, e) => Logic.PageChanged(Convert.ToBoolean(e.NewValue), Model);
        }

        public MainView HideTangent()
        {
            ViewModel.VisibilityTangent = System.Windows.Visibility.Hidden;
            Width = 848;
            return this;
        }

        public Model.MainModel Model => ViewModel.PublicModel;
    }
}
