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
        public event EventHandler<EventArgs> Closing;
        public NewMessage()
        {
            InitializeComponent();
            Message_Border.Visibility = Visibility.Collapsed;

            Background_Border.MouseDown += HideMessage;
        }

        private void HideMessage(object sender, RoutedEventArgs e)
        {
            Message_Border.Visibility = Visibility.Collapsed;
            Closing?.Invoke(this, null);
        }

        public void ShowMessage()
        {
            if (Body_StackPanel.Children.Count > 0)
                Body_StackPanel.Visibility = Visibility.Visible;
            else
                Body_StackPanel.Visibility = Visibility.Collapsed;
            Message_Border.Visibility = Visibility.Visible;
        }

        #region dp string Text
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
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
            message.Text_TextBlock.Visibility = Visibility.Visible;
        }
        #endregion

        #region dp UIElement Body
        public static readonly DependencyProperty BodyProperty = DependencyProperty.Register(
            "Body",
            typeof(UIElement),
            typeof(NewMessage),
            new UIPropertyMetadata(null,
                new PropertyChangedCallback(BodyChanged)));

        public UIElement Body
        {
            get { return (UIElement)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        private static void BodyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            NewMessage message = (NewMessage)depObj;
            if (args.NewValue is UIElement elem)
                message.Body_StackPanel.Children.Add(elem);
        }
        #endregion
    }
}
