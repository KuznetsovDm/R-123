using R123.View;

namespace R123.Learning
{
    class WorkingCapacityTest
    {
        public System.Func<bool>[] Conditions { get; private set; }
        public string[] TextLearning { get; private set; }

        public WorkingCapacityTest()
        {
            Conditions = new System.Func<bool>[6];

            Conditions[0] = () => { return true; };
            Conditions[1] = () => { return true; };
            Conditions[2] = () => { return Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex; };
            Conditions[3] = () => { return Options.Encoders.Noise.Value == 0.1m; };
            Conditions[4] = () => {
                return Options.Switchers.Power.Value == State.on &&
                       Options.Switchers.Scale.Value == State.on;
            };
            Conditions[5] = () => { return Options.PositionSwitchers.Voltage.Value == 0; };



            TextLearning = new string[5];

            TextLearning[0] = "Шлемофон...";
            TextLearning[1] = "Симплекс...";
            TextLearning[2] = "Шумы...";
            TextLearning[3] = "Питание...";
            TextLearning[4] = "Напряжение... в положение \"Работа 1\"";
        }
    }
}
