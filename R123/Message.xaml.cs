using System;
using System.Windows;

namespace R123
{
    /// <summary>
    /// Логика взаимодействия для Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        public Message(string message, bool cancelled)
        {
            InitializeComponent();
            textblock.Text = message;
            if (cancelled) {
                wrapPanel.Children[1].Visibility = Visibility.Visible;
            }
            
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
