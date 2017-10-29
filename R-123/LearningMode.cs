using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace R_123.View
{
    public class LearningMode
    {
        private const int START = -1;
        private const int WORK_MODE = 0;
        private const int NOISE = 1;
        private const int VOLTAGE_CONTROL = 2;
        private const int POWER = 3;
        private const int SCALE = 4;
        private const int VOLUME = 5;
        private const int RANGE = 6;
        private const int CLAMP_ON = 7;
        private const int FREQUENCY = 8;
        private const int CLAMP_OFF = 9;
        private const int SUBFREQUENCY = 10;
        private const int ATHENA = 11;
        private const int END = 12;

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
            "Установите рабочую частоту",
            "Зафиксируйте фиксатор - 1 ",
            "Установите поддиапазон",
            "Расфиксируйте, настройте, зафиксируйте"
        };

        private static Image[] images = {
            Options.PositionSwitchers.WorkMode.Image, // Переключатель рода работы "СИМПЛЕКС - Д.ПРИЕМ"
            Options.Encoders.Noise.Image, // Ручка регулятора шума - "ШУМЫ"
            Options.PositionSwitchers.Voltage.Image, // Переключатель "КОНТРОЛЬ НАПРЯЖЕНИЙ"
            Options.Switchers.Power.Image, // Тумблер включения питания радиостанции "ПИТАНИЕ ВКЛ.-ВЫКЛ."
            Options.Switchers.Scale.Image, // Тумблер включения лампочки освещения шкалы "ШКАЛА ВКЛ.-ВЫКЛ."
            Options.Encoders.Volume.Image, // Ручка регулятора громкости - "ГРОМКОСТЬ"
            Options.PositionSwitchers.Range.Image,//Переключатель "ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН"
            null,// Фиксатор барабана
            Options.Encoders.Frequency.Image,// Ручка "УСТАНОВКА ЧАСТОТЫ"
            null,// Фиксатор барабана
            Options.Switchers.SubFixFrequency[0].Image,// Тумблеры переключения поддиапазонов фиксированных частот
            Options.AthenaDisplay.image// Фиксатор ручки "НАСТРОЙКА АНТЕННЫ"
        };

        private Canvas learning;
        private RectangleGeometry rect1;
        private RectangleGeometry rect2;
        private int counter = 0;
        private bool learningMode = false;
        private TextBlock messageBlock;

        public LearningMode(Canvas learning, RectangleGeometry rect1, RectangleGeometry rect2, TextBlock messageBlock, Image baraban)
        {
            this.learning = learning;
            this.rect1 = rect1;
            this.rect2 = rect2;
            this.messageBlock = messageBlock;
            images[CLAMP_ON] = baraban;
            images[CLAMP_OFF] = baraban;

            Options.PositionSwitchers.WorkMode.ValueChanged += WorkMode_ValueChanged;
            Options.Encoders.Noise.ValueChanged += Noise_ValueChanged;
            Options.PositionSwitchers.Voltage.ValueChanged += Voltage_ValueChanged;
            Options.Switchers.Power.ValueChanged += Power_ValueChanged;
            Options.Switchers.Scale.ValueChanged += Scale_ValueChanged;
            Options.Encoders.Volume.ValueChanged += Volume_ValueChanged;
            Options.PositionSwitchers.Range.ValueChanged += Range_ValueChanged;
            Options.Clamp[0].ValueChanged += Clamp_ValueChanged;
            Options.Encoders.Frequency.ValueChanged += Frequency_ValueChanged;
            Options.Switchers.SubFixFrequency[0].ValueChanged += SubFixFrequency_ValueChanged;
        }

        #region Обработчики

        private void CounterChanger(int nextValue, Func<bool> condition)
        {
            if (learningMode) {
                if (condition()) counter = nextValue;
                else counter = nextValue - 1;
            }
        }

        private void WorkMode_ValueChanged() => CounterChanger(NOISE, () => Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex);

        private void Noise_ValueChanged() => CounterChanger(VOLTAGE_CONTROL, () => Options.Encoders.Noise.Value == 0.0m);

        private void Voltage_ValueChanged() => CounterChanger(POWER, () => Options.PositionSwitchers.Voltage.Value == 0);

        private void Power_ValueChanged() => CounterChanger(SCALE, () => Options.Switchers.Power.Value == State.on);

        private void Scale_ValueChanged() => CounterChanger(VOLUME, () => Options.Switchers.Scale.Value == State.on);

        private void Volume_ValueChanged() => CounterChanger(RANGE, () => Options.Encoders.Volume.Value == 1.0m);

        private void Range_ValueChanged() => CounterChanger(CLAMP_ON, () => Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency1);

        private void Clamp_ValueChanged()
        {
            if (counter == CLAMP_ON || counter == FREQUENCY)
                CounterChanger(FREQUENCY, () => Options.Clamp[0].Value == 1.0m);
            else if (counter == CLAMP_OFF || counter == SUBFREQUENCY)
                CounterChanger(SUBFREQUENCY, () => Options.Clamp[0].Value == 0.0m);
        }

        private void Frequency_ValueChanged()
        {
            if (counter == FREQUENCY)
                CounterChanger(CLAMP_OFF, () => true);
        }

        private void SubFixFrequency_ValueChanged()
        {
            CounterChanger(ATHENA, () => true);
        }

        #endregion
        public void OnStartClick(object sender, RoutedEventArgs args)
        {
            OnStart(Key.L);
        }

        public void OnStopClick(object sender, RoutedEventArgs args)
        {
            OnStart(Key.Escape);
        }
        private void OnStart(Key key)
        {
            switch (key) {
                // Запуск режима обучения
                case Key.L: {
                        if (!learningMode) {
                            learningMode = true;
                            Panel.SetZIndex(learning, 1);
                            counter = START;
                            goto case Key.Enter;
                        }
                        break;
                    }
                // Проверка выполнения условия
                case Key.Enter: {
                        if (learningMode) {
                            if (START < counter && counter < END) {
                                Image image = images[counter];
                                if (image != null) {
                                    double left = Canvas.GetLeft(image);
                                    double top = Canvas.GetTop(image);
                                    double width = image.ActualWidth;
                                    double height = image.ActualHeight;

                                    rect1.Rect = new Rect(left - width * 0.10, top - height * 0.15, width * 1.3, height * 1.3);

                                    if (counter == FREQUENCY)
                                        rect2.Rect = new Rect(300, 0, 150, 100);
                                    else
                                        rect2.Rect = new Rect();

                                    if (messageBlock != null) {
                                        messageBlock.VerticalAlignment = VerticalAlignment.Center;
                                        messageBlock.HorizontalAlignment = HorizontalAlignment.Center;
                                        messageBlock.Text = $"{messages[counter]}{Environment.NewLine}"
                                            + $"Для продолжения нажмите <Enter>{Environment.NewLine}"
                                            + $"Для выхода из режима обучения нажмите <ESC>.";
                                    }
                                }

                                if (counter > SUBFREQUENCY) {
                                    counter++;
                                }
                            }
                            else if (counter == START) {
                                messageBlock.VerticalAlignment = VerticalAlignment.Stretch;
                                messageBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                                messageBlock.Text = $"Вы вошли в режим обучения.{Environment.NewLine}" +
                                    $"Для выхода из режима обучения нажмите <ESC>.{Environment.NewLine}" +
                                    "Для того, чтобы перейти к следующему шагу, нажмите <Enter>.";
                                counter = WORK_MODE;
                            }
                            else if (counter == END) {
                                rect1.Rect = new Rect();
                                messageBlock.VerticalAlignment = VerticalAlignment.Stretch;
                                messageBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                                messageBlock.Text = $"Вы прошли режим обучения, поздравляем!{Environment.NewLine}" +
                                    $"Для выхода из режима обучения нажмите <ESC>.{Environment.NewLine}" +
                                    "Для того, чтобы начать режим обучения заново, нажмите <Enter>.";
                                counter = START;
                            }
                        }
                        break;
                    }
                // Выход из режима обучения
                case Key.Escape: {
                        if (learningMode) {
                            learningMode = false;
                            Panel.SetZIndex(learning, -1);
                            rect1.Rect = new Rect();
                            messageBlock.Text = "";
                            counter = WORK_MODE;
                        }
                        break;
                    }
                default: break;
            }
        }
        public void Start(object sender, KeyEventArgs args)
        {
            OnStart(args.Key);
        }

    }
}