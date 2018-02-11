using System.Windows.Input;

namespace R123.NewRadio.View
{
    public class Tone : Switch
    {
        public Tone() : base("/Files/Images/ToneOff.png")
        {
            MouseDown += TheImage_MouseDown;
            MouseUp += TheImage_MouseUp;
        }

        private void TheImage_MouseDown(object sender, MouseButtonEventArgs e) =>
            RequestChangeValueCommand?.Execute(true);

        private void TheImage_MouseUp(object sender, MouseButtonEventArgs e) =>
            RequestChangeValueCommand?.Execute(false);

        protected override string GetSourse() => $"/Files/Images/Tone{(CurrentValue ? "On" : "Off")}.png";
    }
}
