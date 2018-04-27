using System.Windows.Input;

namespace R123.Radio.View
{
    public class Tangent : Switch
    {
        public Tangent() : base("/Files/Images/tangenta_prm.png")
        {
            MouseDown += (s, e) => CallChangeValue(true);
            MouseUp += (s, e) => CallChangeValue(false);

            MainWindow.Instance.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            MainWindow.Instance.KeyDown -= OnKeyDown;
            MainWindow.Instance.KeyUp += OnKeyUp;
            if (Keyboard.IsKeyDown(Key.Space))
                CallChangeValue(true);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            MainWindow.Instance.KeyUp -= OnKeyUp;
            MainWindow.Instance.KeyDown += OnKeyDown;
            if (Keyboard.IsKeyUp(Key.Space))
                CallChangeValue(false);
        }

        private void CallChangeValue(bool newValue)
        {
            RequestChangeValueCommand?.Execute(newValue);
            if (newValue)
            {
                //Mouse.Capture(this);
                //LostMouseCapture += OnLostCapture;
                System.Diagnostics.Trace.WriteLine("subscribe");
            }
            else
            {
                //LostMouseCapture -= OnLostCapture;
                //Mouse.Capture(null);
                System.Diagnostics.Trace.WriteLine("unsubscribe");
            }
        }

        void OnLostCapture(object sender, MouseEventArgs e)
        {
            //LostMouseCapture -= OnLostCapture;
            CallChangeValue(false);
            System.Diagnostics.Trace.WriteLine("lost capture");
        }

        protected override string GetSourse() => $"/Files/Images/tangenta_{(CurrentValue ? "prd" : "prm")}.png";
    }
}
