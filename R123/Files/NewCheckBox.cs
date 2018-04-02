using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace R123.Files
{
    class NewCheckBox : Image
    {
        public bool IsChecked {
            set
            {
                if (value) {
                    Source = new BitmapImage(new Uri("/Files/Images/yes.png", UriKind.Relative));
                } else {
                    Source = new BitmapImage(new Uri("/Files/Images/no.png", UriKind.Relative));
                }
            }
        }
    }
}
