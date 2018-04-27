using System;
using System.Windows;
using System.Windows.Controls;

namespace R123.AdditionalWindows
{
    /// <summary>
    /// Логика взаимодействия для NewMessage.xaml
    /// </summary>
    public partial class NewMessage : UserControl
    {
        public NewMessage()
        {
            InitializeComponent();
            Message_Border.Visibility = Visibility.Collapsed;

            Message_Border.MouseDown += HideMessage;
        }

        private void HideMessage(object sender, RoutedEventArgs e)
        {
            Message_Border.Visibility = Visibility.Collapsed;
        }

        public void ShowMessage()
        {
            Message_Border.Visibility = Visibility.Visible;
        }

        #region dp string Text
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Angle",
            typeof(string),
            typeof(NewMessage),
            new UIPropertyMetadata("",
                new PropertyChangedCallback(TextChanged)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            NewMessage message = (NewMessage)depObj;
            message.Text_TextBlock.Text = Convert.ToString(args.NewValue);
        }
        #endregion
    }
}
