using System.Windows.Controls;

namespace R123.View
{
    /// <summary>
    /// Показывает номер текущей фиксированной частоты.
    /// </summary>
    public class FixedFrequencyDisplay
    {
        /// <summary>
        /// Изображение дисплея
        /// </summary>
        public Image image;
        /// <param name="image">Изображение дисплея</param>
        public FixedFrequencyDisplay(Image image)
        {
            this.image = image;

            Options.PositionSwitchers.Range.ValueChanged += Update;
            Options.Switchers.Power.ValueChanged += Update;

            Update();
        }
        /// <summary>
        /// Обновить дисплей при изменении фиксированной частоты и при включении питания.
        /// </summary>
        private void Update()
        {
            const string nameImage = "FixedFrequencyDisplay";
            int numberImage = 0;

            if (Options.Switchers.Power.Value == State.off)
            {
                // если выключено питание
                numberImage = 0;
            }
            else if (Options.PositionSwitchers.Range.Value <= RangeSwitcherValues.FixFrequency4)
            {
                // иначе, если включена фиксированная частота
                numberImage = (int)Options.PositionSwitchers.Range.Value + 1;
            }
            else
            {
                // иначе, если включен плавный поддиапазон
                numberImage = 0;
            }

            System.Windows.Media.Imaging.BitmapImage bi3 = new System.Windows.Media.Imaging.BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new System.Uri("/Files/Images/" + nameImage + numberImage + ".gif", System.UriKind.Relative);
            bi3.EndInit();
            image.Source = bi3;
        }
    }
}
