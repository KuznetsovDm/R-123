using System.Windows.Controls;

namespace R_123.View
{
    /// <summary>
    /// Дисплей, отображающий номер текущего поддиапазона.
    /// </summary>
    public class SubFrequencyDisplay
    {
        public Image image;
        /// <param name="image">Изображение дисплея поддиапазонов.</param>
        public SubFrequencyDisplay(Image image)
        {
            this.image = image;

            Options.PositionSwitchers.Range.ValueChanged += Update;
            Options.Switchers.Power.ValueChanged += Update;

            for (int i = 0; i < Options.Switchers.SubFixFrequency.Length; i++)
            {
                Options.Switchers.SubFixFrequency[i].ValueChanged += Update;
            }
            Update();
        }
        /// <summary>
        /// Обновить дисплей при включении тумблера питания и переключателей поддиапазона.
        /// </summary>
        public void Update()
        {
            const string nameImage = "SubFrequency";
            int numberImage = 0;

            if (Options.Switchers.Power.Value == State.off) // если питание выключено.
                numberImage = 0;
            else                                            // иначе если питание включено.
            {
                // иначе если выбран плавный поддиапазон.
                if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency1)
                    numberImage = 1;
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2)
                    numberImage = 2;
                else
                {
                    // иначе если выбрана фиксированная частота.
                    int number = (int)Options.PositionSwitchers.Range.Value;
                    SubFrequency currentSubFrequency = Options.Switchers.SubFixFrequency[number].Value;
                    if (currentSubFrequency == SubFrequency.One)
                        numberImage = 1;
                    else
                        numberImage = 2;
                }
            }

            System.Windows.Media.Imaging.BitmapImage bi3 = new System.Windows.Media.Imaging.BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new System.Uri("/Files/Images/" + nameImage + numberImage + ".gif", System.UriKind.Relative);
            bi3.EndInit();
            image.Source = bi3;
        }
    }
}
