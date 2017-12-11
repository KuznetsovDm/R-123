using System;
using System.Windows.Controls;

namespace R123.Radio
{
    public class Switch : IPropertySwitcher
    {
        public event EventHandler<ValueChangedEventArgsSwitcher> ValueChanged;
        private View.SwitchElement element;

        public Switch(Image image)
        {
            element = new View.SwitchElement(image);
            element.ValueChanged += (object sender, ValueChangedEventArgs<bool> e) => OnValueChanged();
        }

        public bool Value
        {
            get => element.Value;
        }

        public Image Image => element.image;

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgsSwitcher(Value));
        }
    }
    public class NumberedSwitch : Switch, IPropertyNumberedSwitcher
    {
        public new event EventHandler<ValueChangedEventArgsNumberedSwitcher> ValueChanged;
        private int number;

        public NumberedSwitch(Image image, int number) : base(image)
        {
            this.number = number;
        }

        protected override void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgsNumberedSwitcher(Value, number));
        }
    }
}
