using System;
using System.Windows;
using System.Windows.Controls;

namespace R123.AdditionalWindows
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

        public Message(StackPanel stackPanel, bool cancelled)
        {
            InitializeComponent();
            grid.Children.Add(stackPanel);

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
