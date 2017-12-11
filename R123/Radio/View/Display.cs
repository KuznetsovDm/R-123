using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace R123.Radio.View
{
    class Display
    {
        private Image image;
        private int currentValue;
        private int maxValue;
        private bool visible;
        private string name;

        public Display(Image image, int maxValue, string nameImage)
        {
            this.image = image;
            this.maxValue = maxValue;
            currentValue = 1;
            visible = false;
            name = nameImage;
        }

        public int Value
        {
            get => currentValue;
            set
            {
                if (value > maxValue || value < 0)
                    throw new ArgumentException("Некорректный номер изображения!");

                currentValue = value;
                Update();
            }
        }

        public bool Visibility
        {
            get => visible;
            set
            {
                visible = value;
                Update();
            }
        }

        private void Update()
        {
            int number = visible ? currentValue : 0;
            image.Source = new BitmapImage(new Uri($"/Files/Images/{name}{number}.gif", UriKind.Relative));
        }
    }
}
