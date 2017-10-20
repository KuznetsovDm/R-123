using System.Windows.Controls;

namespace R_123.View
{
    abstract class Switcher : ImagesControl
    {
        private bool currentValue;
        public Switcher(Image image, bool defValue = false) : base(image)
        {
            currentValue = defValue;
            Source = currentValue;

            cursorImages = CursorImages.mouseIconLeftCenter;

            image.MouseWheel += Image_MouseWheel;
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
        }
        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => 
            CurrentValue = !CurrentValue;
        private void Image_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && Value == false)
                CurrentValue = true;
            else if (e.Delta < 0 && Value == true)
                CurrentValue = false;
        }
        public bool Value => currentValue;
        protected bool CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                Source = value;
                PlaySound();
            }
        }
        private void PlaySound()
        {
            try
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                player.SoundLocation = @"C:\Users\DK\Documents\R-123\R-123\R-123\sounds\PositionSwitcher.wav";
                player.Load();
                player.Play();
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine("Ошибка воспроизведения звукового файла");
            }
        }
    }
}
