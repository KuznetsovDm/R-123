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

            IsVisibleChanged += (s, e) => Logic.PageChanged2(Convert.ToBoolean(e.NewValue), Radio.Model);
        }
    }
}
