using System.Windows.Input;

namespace R123.Radio.View
{
    public class Tangent : Switch
    {
        public Tangent() : base("/Files/Images/tangenta_prm.png")
        {
            MouseDown += (s, e) => CallChangeValue(true);
            MouseUp += (s, e) => CallChangeValue(false);

            //Keyboard.AddKeyDownHandler()

            MainWindow.Instance.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            MainWindow.Instance.KeyDown -= OnKeyDown;
            MainWindow.Instance.KeyUp += OnKeyUp;
            if (e.Key == Key.Space)
                CallChangeValue(true);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            MainWindow.Instance.KeyUp -= OnKeyUp;
            MainWindow.Instance.KeyDown += OnKeyDown;
            if (e.Key == Key.Space)
                CallChangeValue(false);
        }

        private void CallChangeValue(bool newValue)
        {
            RequestChangeValueCommand?.Execute(newValue);
        }

        void OnLostCapture(object sender, MouseEventArgs e)
        {
            //LostMouseCapture -= OnLostCapture;
            CallChangeValue(false);
        }

        protected override string GetSourse() => $"/Files/Images/tangenta_{(CurrentValue ? "prd" : "prm")}.png";
    }
}
