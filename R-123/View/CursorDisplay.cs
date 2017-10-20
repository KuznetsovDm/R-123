using System.Windows.Controls;
using System.Windows.Media;

namespace R_123.View
{
    enum CursorImages { mouseIcon, mouseIconCenter, mouseIconLeft, mouseIconLeftCenter };
    class CursorDisplay
    {
        private Image imageCursor;
        private TextBlock textSpace, textCtrl;
        public CursorDisplay(Image imageCursor, TextBlock textSpace, TextBlock textCtrl)
        {
            this.imageCursor = imageCursor;
            this.textSpace = textSpace;
            this.textCtrl = textCtrl;

            Options.Switchers.Power.ValueChanged += SetSpaceTextBlock;
            Options.PositionSwitchers.WorkMode.ValueChanged += SetSpaceTextBlock;
        }
        public void SetCursor(CursorImages cur)
        {
            string name = System.Enum.GetName(typeof(CursorImages), cur);
            System.Windows.Media.Imaging.BitmapImage bi3 = new System.Windows.Media.Imaging.BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new System.Uri("/Files/Images/mouseIcon/" + name + ".png", System.UriKind.Relative);
            bi3.EndInit();
            imageCursor.Source = bi3;
        }
        private void SetSpaceTextBlock()
        {
            if (Options.Switchers.Power.Value == State.on && 
                Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex)
                textSpace.Foreground = new SolidColorBrush(Colors.Green); 
            else
                textSpace.Foreground = new SolidColorBrush(Colors.Black);
        }
        public void SetCtrlTextBlock(bool green)
        {
            if (green)
                textCtrl.Foreground = new SolidColorBrush(Colors.Green);
            else
                textCtrl.Foreground = new SolidColorBrush(Colors.Black);
        }
    }
}
