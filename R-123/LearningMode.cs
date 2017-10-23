using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace R_123.View
{
    public class LearningMode
    {
        private static readonly string[] messages =
        {
            "Установите режим \"СИМПЛЕКС\"",
            "Поверните против часовой стрелки до упора",
            "Установите \"Работа - 1\"",
            "Установите в положение \"ВКЛ\"",
            "Установите в положение \"ВКЛ\"",
            "Поверните по часовой стрелке до упора",
            "Установите в положение \"1\"",
            "Расфиксируйте фиксатор - 1",
            "Установите рабочую частоту и зафиксируйте фиксатор - 1 ",
            "Установите поддиапазон",
            "Расфиксируйте, настройте, зафиксируйте"
        };


        private static Image[] images = {
            Options.PositionSwitchers.WorkMode.Image, // Переключатель рода работа "СИМПЛЕКС - Д.ПРИЕМ"
            Options.Encoders.Noise.Image, // Ручка регулятора шума - "ШУМЫ"
            Options.PositionSwitchers.Voltage.Image, // Переключатель "КОНТРОЛЬ НАПРЯЖЕНИЙ"
            Options.Switchers.Power.Image, // Тумблер включения питания радиостанции "ПИТАНИЕ ВКЛ.-ВЫКЛ."
            Options.Switchers.Scale.Image, // Тумблер включения лампочки освещения шкалы "ШКАЛА ВКЛ.-ВЫКЛ."
            Options.Encoders.Volume.Image, // Ручка регулятора громкости - "ГРОМКОСТЬ"
            Options.PositionSwitchers.Range.Image,//Переключатель "ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН"
            null,// Крышка люка барабана
            Options.Encoders.Frequency.Image,// Ручка "УСТАНОВКА ЧАСТОТЫ"
            Options.Display.SubFrequency.image,// Тумблеры переключения поддиапазонов фиксированных частот
            Options.AthenaDisplay.image// Фиксатор ручки "НАСТРОЙКА АНТЕННЫ"
        };

        private Canvas learning;
        private RectangleGeometry rect;
        private int counter = 0;
        private bool learningMode = false;
        private TextBlock textBlock;

        public LearningMode(Canvas learning, RectangleGeometry rect, TextBlock textBlock, Image baraban)
        {
            this.learning = learning;
            this.rect = rect;
            this.textBlock = textBlock;
            images[7] = baraban;

            Options.PositionSwitchers.WorkMode.ValueChanged += WorkMode_ValueChanged;
            Options.Encoders.Noise.ValueChanged += Noise_ValueChanged;
            Options.PositionSwitchers.Voltage.ValueChanged += Voltage_ValueChanged;
            Options.Switchers.Power.ValueChanged += Power_ValueChanged;
            Options.Switchers.Scale.ValueChanged += Scale_ValueChanged;
            Options.Encoders.Volume.ValueChanged += Volume_ValueChanged;
            Options.PositionSwitchers.Range.ValueChanged += Range_ValueChanged;

            Options.Encoders.Frequency.ValueChanged += Frequency_ValueChanged;
        }
        #region Обработчики

        private void CounterChanger(int nextValue, Func<bool> condition)
        {
            if (learningMode) {
                if (learningMode && condition()) counter = nextValue;
                else counter = nextValue - 1;
            }
        }

        private void WorkMode_ValueChanged()
        {
            CounterChanger(1, () => Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex);
        }

        private void Noise_ValueChanged()
        {
            CounterChanger(2, () => Options.Encoders.Noise.Value == 0.0m);
        }

        private void Voltage_ValueChanged()
        {
            CounterChanger(3, () => Options.PositionSwitchers.Voltage.Value == 0);
        }

        private void Power_ValueChanged()
        {
            CounterChanger(4, () => Options.Switchers.Power.Value == State.on);
        }

        private void Scale_ValueChanged()
        {
            CounterChanger(5, () => Options.Switchers.Scale.Value == State.on);
        }

        private void Volume_ValueChanged()
        {
            CounterChanger(6, () => Options.Encoders.Volume.Value == 1.0m);
        }

        private void Range_ValueChanged()
        {
            //   CounterChanger(7, () => Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency1);
        }

        private void Frequency_ValueChanged()
        {
        }

        #endregion
        public void Start(object sender, KeyEventArgs args)
        {
            switch (args.Key) {
                // Запуск режима обучения
                case Key.F1: {
                        if (!learningMode) {
                            learningMode = true;
                            Panel.SetZIndex(learning, 1);
                            counter = 0;
                        }
                        break;
                    }
                // Проверка выполнения условия
                case Key.Enter: {
                        if (learningMode) {
                            if (counter < images.Length) {
                                Image image = images[counter];
                                if (image != null) {
                                    double left = Canvas.GetLeft(image);
                                    double top = Canvas.GetTop(image);
                                    double width = image.ActualWidth;
                                    double height = image.ActualHeight;

                                    rect.Rect = new Rect(left - width * 0.15, top - height * 0.15, width * 1.3, height * 1.3);

                                    if (textBlock != null)
                                        textBlock.Text = messages[counter];
                                }
                                if (counter > 5) counter++;

                            }
                            else counter = 0;
                        }
                        break;
                    }
                // Выход из режима обучения
                case Key.Escape: {
                        if (learningMode) {
                            learningMode = false;
                            Panel.SetZIndex(learning, -1);
                            rect.Rect = new Rect();
                            counter = 0;
                        }
                        break;
                    }
                default: break;
            }
        }
    }
}