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

namespace R123.Radio.View
{
    /// <summary>
    /// Логика взаимодействия для RadioPage.xaml
    /// </summary>
    public partial class RadioPage : Page
    {
        public Radio Radio { get; private set; }
        public RadioPage()
        {
            InitializeComponent();
            Radio = new Radio(this);
        }
        public void HideTangent()
        {
            tangenta_Image.Visibility = Visibility.Hidden;
            Width = 848;
        }
    }
}
