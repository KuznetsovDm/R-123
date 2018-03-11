using System.Windows.Input;

namespace R123.Radio.View
{
    public class Tangent : Switch
    {
        public Tangent() : base("/Files/Images/tangenta_prm.png")
        {
            MouseDown += (s, e) => CallChangeValue(true, true);
            MouseUp += (s, e) => CallChangeValue(true, false);

            MainWindow.Instance.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            MainWindow.Instance.KeyDown -= OnKeyDown;
            MainWindow.Instance.KeyUp += OnKeyUp;
            CallChangeValue(Keyboard.IsKeyDown(Key.Space), true);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            MainWindow.Instance.KeyUp -= OnKeyUp;
            MainWindow.Instance.KeyDown += OnKeyDown;
            CallChangeValue(Keyboard.IsKeyUp(Key.Space), false);
        }

        private void CallChangeValue(bool condition, bool newValue)
        {
            if (condition && CurrentValue != newValue)
                RequestChangeValueCommand?.Execute(newValue);
        }

        protected override string GetSourse() => $"/Files/Images/tangenta_{(CurrentValue ? "prd" : "prm")}.png";
    }
}
