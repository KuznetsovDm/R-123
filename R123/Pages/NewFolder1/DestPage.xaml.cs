using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для DestPage.xaml
    /// </summary>
    public partial class DestPage : Page
    {
        public DestPage()
        {
            InitializeComponent();

            byte[] bytes = File.ReadAllBytes("../../Files/dest.txt");
            string result = Encoding.UTF8.GetString(Encoding.Convert(Encoding.GetEncoding("windows-1251"), Encoding.UTF8, bytes));
            content.Text = result;
        }
    }
}
