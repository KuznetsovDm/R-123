using System;
using R123.View;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace R123.Learning
{
    //public interface IRadioTask
    //{
    //    decimal Frequency { get; }
    //    decimal Antenna { get; }
    //    bool Tone { get; }
    //    WorkModeValue WorkMode { get; }
    //    event EventHandler<EventArgs> AllConditionsDone;
    //    bool CheckTone(bool toneState);
    //    bool CheckFrequency(decimal frequencyMachine);
    //    bool CheckAntenna(decimal antennaMachine);
    //    bool CheckPower();
    //    bool CheckWorkMode();
    //    void CheckAll();
    //}
    public class RadioFrequencyTask
    {
        public decimal Frequency { get; private set; }
        public decimal Antenna { get; private set; }
        public WorkModeValue WorkMode { get; private set; }
        public bool Tone { get; set; }
        public bool PlayTone { get; private set; }

        public RadioFrequencyTask(decimal frequency, decimal antenna, WorkModeValue workMode, bool playTone)
        {
            Frequency = frequency;
            Antenna = antenna;
            WorkMode = workMode;
            PlayTone = playTone;
        }

        public event EventHandler<EventArgs> AllConditionsDone;

        public static bool CheckDecimal(decimal a, decimal b, decimal delta)
        {
            if (Math.Abs((double)(a - b)) < (double)delta)
                return true;
            else
                return false;
        }
        public bool CheckFrequency(decimal frequencyMachine)
        {
            return CheckDecimal(frequencyMachine, Frequency, 0.001M);
        }
        public bool CheckAntenna(decimal antennaMachine)
        {
            return (antennaMachine > 0.8M) ? true : false;
        }
        public bool CheckPower()
        {
            return Options.Switchers.Power.Value == State.on;
        }
        public bool CheckWorkMode()
        {
            if (Options.PositionSwitchers.WorkMode.Value == WorkMode)
                return true;
            else
                return false;
        }
        public bool CheckTone()
        {
            if (!PlayTone)
                return true;
            else
                return Tone;
        }

        protected void InvokeAllConditionsDone(object sender)
        {
            AllConditionsDone?.Invoke(sender, null);
        }

        public virtual void CheckAll()
        {
            if (CheckAntenna((decimal)Options.Encoders.AthenaDisplay.Value) && CheckFrequency(Options.Encoders.Frequency.Value) &&
                CheckPower() && CheckWorkMode() && CheckTone())
                AllConditionsDone?.Invoke(this, null);
        }
    }

    public class RadioFixedFrequencyTask : RadioFrequencyTask
    {
        RangeSwitcherValues fixFrequency;
        public RadioFixedFrequencyTask(decimal frequency, decimal antenna,
                                        WorkModeValue workMode, bool playTone,
                                        RangeSwitcherValues fixFrequency) :
                                        base(frequency, antenna, workMode, playTone)
        {
            this.fixFrequency = fixFrequency;
        }

        bool CheckDecimal(decimal a, decimal b)
        {
            return RadioFrequencyTask.CheckDecimal(a, b, 0.001m);
        }

        public bool CheckFixFrequency()
        {
            if (fixFrequency == RangeSwitcherValues.FixFrequency1)
            {
                if (Frequency == 35.75M)
                {
                    if (CheckDecimal(Properties.Settings.Default.FixedFrequency1_1,Frequency) ||
                       CheckDecimal(Properties.Settings.Default.FixedFrequency1_2, Frequency))
                        return true;
                    else
                        return false;
                }
                else
                if (IsFirstSubFrequency())
                    return CheckDecimal(Properties.Settings.Default.FixedFrequency1_1, Frequency);
                else
                    return CheckDecimal(Properties.Settings.Default.FixedFrequency1_2, Frequency);
            }
            else if (fixFrequency == RangeSwitcherValues.FixFrequency2)
            {
                if (Frequency == 35.75M)
                {
                    if (CheckDecimal(Properties.Settings.Default.FixedFrequency2_1, Frequency) ||
                       CheckDecimal(Properties.Settings.Default.FixedFrequency2_2, Frequency))
                        return true;
                    else
                        return false;
                }
                else
                if (IsFirstSubFrequency())
                    return CheckDecimal(Properties.Settings.Default.FixedFrequency2_1, Frequency);
                else
                    return CheckDecimal(Properties.Settings.Default.FixedFrequency2_2, Frequency);
            }
            else if (fixFrequency == RangeSwitcherValues.FixFrequency3)
            {
                if (Frequency == 35.75M)
                {
                    if (CheckDecimal(Properties.Settings.Default.FixedFrequency3_1, Frequency) ||
                       CheckDecimal(Properties.Settings.Default.FixedFrequency3_2, Frequency))
                        return true;
                    else
                        return false;
                }
                else
                if (IsFirstSubFrequency())
                    return CheckDecimal(Properties.Settings.Default.FixedFrequency3_1, Frequency);
                else
                    return CheckDecimal(Properties.Settings.Default.FixedFrequency3_2, Frequency);
            }
            else if (fixFrequency == RangeSwitcherValues.FixFrequency4)
            {
                if (Frequency == 35.75M)
                {
                    if (CheckDecimal(Properties.Settings.Default.FixedFrequency4_1, Frequency) ||
                       CheckDecimal(Properties.Settings.Default.FixedFrequency4_2, Frequency))
                        return true;
                    else
                        return false;
                }
                else
                if (IsFirstSubFrequency())
                    return CheckDecimal(Properties.Settings.Default.FixedFrequency4_1, Frequency);
                else
                    return CheckDecimal(Properties.Settings.Default.FixedFrequency4_2, Frequency);
            }
            else return false;
        }

        public override void CheckAll()
        {
            if (CheckAntenna((decimal)Options.Encoders.AthenaDisplay.Value) && CheckFrequency(Options.Encoders.Frequency.Value) &&
                CheckPower() && CheckWorkMode() && CheckTone()
                && CheckFixFrequency())
                InvokeAllConditionsDone(this);
        }

        public bool IsFirstSubFrequency()
        {
            if (Frequency >= 35.75M)
                return false;
            else
                return true;
        }
    }

    public class TaskController
    {
        public RadioFrequencyTask Task { get; private set; }
        private StackPanel panel;
        private DispatcherTimer dispatcherTimer;
        private int timeOut;
        public TaskController(RadioFrequencyTask task, StackPanel panel)
        {
            Options.SetInitialValue(true);
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            timeOut = 0;
            dispatcherTimer.Start();
            Task = task;
            this.panel = panel;
            UpdateInfoTask();
            Options.Encoders.Frequency.ValueChanged += WorkMode_ValueChanged;
            Options.Encoders.AthenaDisplay.ValueChanged += WorkMode_ValueChanged;
            Options.PositionSwitchers.WorkMode.ValueChanged += WorkMode_ValueChanged;
            Options.Tone.ValueChanged += WorkMode_ValueChanged;
            Task.AllConditionsDone += Task_AllConditionsDone;
        }

        private void Task_AllConditionsDone(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            MessageBox.Show("All conditionstDone!");
            Close();
        }

        CheckBox boxFr, boxAn, boxT;
        TextBlock timerBlock;
        private void WorkMode_ValueChanged()
        {
            if (Task.CheckFrequency(Options.Encoders.Frequency.Value))
                boxFr.IsChecked = true;
            else
                boxFr.IsChecked = false;

            if (Task.CheckAntenna((decimal)Options.Encoders.AthenaDisplay.Value))
                boxAn.IsChecked = true;
            else
                boxAn.IsChecked = false;

            if (Options.Switchers.Power.Value == State.on)
            {
                Task.Tone = true;
                boxT.IsChecked = true;
            }
            else
            {
                Task.Tone = false;
                boxT.IsChecked = false;
            }

            Task.CheckAll();
        }

        private void AthenaDisplay_ValueChanged()
        {
            Task.CheckAntenna((decimal)Options.Encoders.AthenaDisplay.Value);
            Task.CheckAll();
        }

        private void Frequency_ValueChanged()
        {
            Task.CheckFrequency((decimal)Options.Encoders.Frequency.Value);
            Task.CheckAll();
        }
        private void UpdateInfoTask()
        {
            while (panel.Children.Count > 0)
                panel.Children.RemoveAt(0);

            StackPanel frequency = new StackPanel();
            frequency.Orientation = Orientation.Horizontal;
            boxFr = new CheckBox();
            boxFr.IsEnabled = false;
            frequency.Children.Add(boxFr);
            TextBlock textFr = new TextBlock();
            textFr.Text = $"1) установите частоту {Task.Frequency} МГц";
            frequency.Children.Add(textFr);
            panel.Children.Add(frequency);


            StackPanel antenna = new StackPanel();
            antenna.Orientation = Orientation.Horizontal;
            boxAn = new CheckBox();
            boxAn.IsEnabled = false;
            antenna.Children.Add(boxAn);
            TextBlock textAn = new TextBlock();
            textAn.Text = "2) настройте антену";
            antenna.Children.Add(textAn);
            panel.Children.Add(antenna);


            StackPanel tone = new StackPanel();
            tone.Orientation = Orientation.Horizontal;
            boxT = new CheckBox();
            boxT.IsEnabled = false;
            tone.Children.Add(boxT);
            TextBlock textT = new TextBlock();
            textT.Text = "3) Нажмите кнопку \"Тон\"";
            tone.Children.Add(textT);
            panel.Children.Add(tone);


            StackPanel timerStack = new StackPanel();
            timerStack.Orientation = Orientation.Horizontal;
            TextBlock textTi = new TextBlock();
            textTi.Text = "Потраченное время: ";
            timerStack.Children.Add(textTi);
            timerBlock = new TextBlock();
            timerBlock.Text = "0";
            timerStack.Children.Add(timerBlock);
            TextBlock sec = new TextBlock();
            sec.Text = " сек.";
            timerStack.Children.Add(sec);
            panel.Children.Add(timerStack);
        }
        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            timeOut++;
            timerBlock.Text = timeOut.ToString();
        }
        public void Close()
        {
            Options.Encoders.Frequency.ValueChanged -= WorkMode_ValueChanged;
            Options.Encoders.AthenaDisplay.ValueChanged -= WorkMode_ValueChanged;
            Options.PositionSwitchers.WorkMode.ValueChanged -= WorkMode_ValueChanged;
            Options.Tone.ValueChanged -= WorkMode_ValueChanged;
            Task.AllConditionsDone -= Task_AllConditionsDone;

            dispatcherTimer.Stop();
        }
    }
}
