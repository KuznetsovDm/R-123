using System;
using R123.Learning;
using R123.Radio.Model;

namespace RadioTask.Model.Chain.Specialized
{
    public class HandlerSpecialized : Handler
    {
        private int buttonsCount = 24;
        private int currentStep = 0;
        private WorkingTest workingTest;
        private MainModel radioModel;

        public Action[] Subscribes { get; private set; }
        public Action[] Unsubscribes { get; private set; }

        public HandlerSpecialized(MainModel model)
        {
            radioModel = model;
        }

        public override void Start()
        {
            workingTest = new WorkingTest(radioModel);
            
            InitializeSubscribes();
            InitializeUnsubscribes();

            Subscribe(currentStep);
            IsWork = true;
        }
        
        
        private void StepCheck(object sender, EventArgs args)
        {
            Console.WriteLine($"Current step:{currentStep}");
            if (currentStep < buttonsCount - 1)
            {
                if (currentStep == 5 || currentStep == 15)
                    workingTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                else if (currentStep == 8)
                {
                    workingTest.RemoveCondition(2); // удаляем проверку шумов
                    workingTest.RemoveCondition(currentStep - 1);
                }
                else if (currentStep == 9)
                {
                    workingTest.RemoveCondition(6); // удаляем проверку 1 поддиапазона
                    workingTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                }
                else if (currentStep == 10)
                    workingTest.RemoveCondition(1); // удаляем проверку симплекса
                else if (currentStep == 13)
                {
                    workingTest.RemoveCondition(10); // удаляем проверку дежурного приема
                    workingTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                }
                else if (currentStep == 18)
                {
                    workingTest.RemoveCondition(9); // удаляем проверку 2 поддиапазона
                    workingTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                }
                else if (currentStep == 22)
                {
                    workingTest.RemoveCondition(18); // удаляем проверку 1 поддиапазона
                    workingTest.RemoveCondition(15); // удаляем проверку настройки антенны
                    workingTest.RemoveCondition(21); // удаляем проверку настройки антенны
                }

                CheckWithAddingCondition(ref currentStep);
            }
            else if (currentStep == buttonsCount - 1)
            {
                workingTest.RemoveCondition(3); // удаляем проверку 1 поддиапазона
                if (!workingTest.CheckCondition(out currentStep))
                    return;

                workingTest.Clear();

                UnsubscribeAll();

                currentStep = 0;
                //End state
                OnAllStepsPassed(new EventArgs());
            }
        }

        private void Subscribe(int index)
        {
            Subscribes[index]?.Invoke();
        }

        private void Unsubscribe(int index)
        {
            Unsubscribes[index]?.Invoke();
        }
        
        private void InitializeSubscribes()
        {
            Subscribes = new Action[24];

            Subscribes[0] = () => radioModel.Tangent.ValueChanged += StepCheck;
            Subscribes[1] = () => radioModel.WorkMode.ValueChanged += StepCheck;
            Subscribes[2] = () => radioModel.Noise.ValueChanged += StepCheck;
            Subscribes[3] = () =>
            {
                radioModel.Scale.ValueChanged += StepCheck;
                radioModel.Power.ValueChanged += StepCheck;
            };
            Subscribes[4] = () => radioModel.Tangent.ValueChanged += StepCheck;
            Subscribes[5] = () => radioModel.Volume.ValueChanged += StepCheck;
            Subscribes[6] = () => radioModel.Range.ValueChanged += StepCheck;
            Subscribes[7] = () => radioModel.Frequency.ValueChanged += StepCheck;
            Subscribes[12] = () => radioModel.Tone.ValueChanged += StepCheck;
            Subscribes[15] = () => radioModel.Antenna.EndValueChanged += AntennaCheck;
            Subscribes[20] = () =>
            {
                radioModel.Clamps[0].ValueChanged += StepCheck;
                radioModel.Clamps[1].ValueChanged += StepCheck;
                radioModel.Clamps[2].ValueChanged += StepCheck;
                radioModel.Clamps[3].ValueChanged += StepCheck;
            };
        }

        private void InitializeUnsubscribes()
        {
            Unsubscribes = new Action[24];

            Unsubscribes[0] = () => radioModel.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[1] = () => radioModel.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[2] = () => radioModel.Noise.ValueChanged -= StepCheck;
            Unsubscribes[3] = () =>
            {
                radioModel.Scale.ValueChanged -= StepCheck;
                radioModel.Power.ValueChanged -= StepCheck;
                radioModel.Frequency.ValueChanged -= StepCheck;
            };
            Unsubscribes[4] = () => radioModel.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[5] = () => radioModel.Volume.ValueChanged -= StepCheck;
            Unsubscribes[6] = () => radioModel.Range.ValueChanged -= StepCheck;
            Unsubscribes[7] = () => radioModel.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[8] = () => radioModel.Noise.ValueChanged -= StepCheck;
            Unsubscribes[9] = () => radioModel.Range.ValueChanged -= StepCheck;
            Unsubscribes[10] = () => radioModel.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[11] = () => radioModel.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[12] = () => radioModel.Tone.ValueChanged -= StepCheck;
            Unsubscribes[13] = () => radioModel.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[14] = () => radioModel.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[15] = () => radioModel.Antenna.ValueChanged -= StepCheck;
            Unsubscribes[16] = () => radioModel.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[17] = () => radioModel.Tone.ValueChanged -= StepCheck;
            Unsubscribes[18] = () => radioModel.Range.ValueChanged -= StepCheck;
            Unsubscribes[19] = () => radioModel.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[20] = () => radioModel.Clamps[0].ValueChanged -= StepCheck;
            Unsubscribes[21] = () => radioModel.Antenna.ValueChanged -= StepCheck;
            Unsubscribes[22] = () => radioModel.Range.ValueChanged -= StepCheck;
            Unsubscribes[23] = () => radioModel.Power.ValueChanged -= StepCheck;
        }
        
        private void AntennaCheck(object sender, EventArgs args)
        {
            if (radioModel.Antenna.Value < 0.8)
                return;

            StepCheck(sender, args);
        }

        private void CheckWithAddingCondition(ref int current)
        {
            if (workingTest.CheckCondition(out currentStep))
            {
                workingTest.AddCondition(currentStep);
                Subscribe(currentStep);
            }
        }
        private void UnsubscribeAll()
        {
            for (int i = 0; i < Unsubscribes.Length; i++)
            {
                Unsubscribe(i);
            }
        }

        public override void Stop()
        {
            workingTest.Clear();

            UnsubscribeAll();

            currentStep = 0;
            IsWork = false;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Handle(Step obj)
        {
            base.Handle(obj);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}