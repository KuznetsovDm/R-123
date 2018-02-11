//namespace R123.Learning
//{
//    class WorkingCapacityTest
//    {
//        public System.Func<bool>[] Conditions { get; private set; }
//        public string[] TextLearning { get; private set; }

//        public WorkingCapacityTest()
//        {
//            Conditions = new System.Func<bool>[25];

//            Conditions[0] = () => { return true; };
//            Conditions[1] = () => { return true; };
//            Conditions[2] = () => { return Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex; };
//            Conditions[3] = () => { return Options.Encoders.Noise.Value == 0.1m; };
//            Conditions[4] = () => {
//                return Options.Switchers.Power.Value == State.on &&
//                       Options.Switchers.Scale.Value == State.on;
//            };
//            Conditions[5] = () => { return Options.PositionSwitchers.Voltage.Value == 0; };
//            Conditions[6] = () => { return Options.Encoders.Volume.Value == 1m; };
//            Conditions[7] = () => { return Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency1; };
//            Conditions[8] = () => { return true; };
//            Conditions[9] = () => { return true; };
//            Conditions[10] = () => { return Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2; };
//            Conditions[11] = () => { return Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Acceptance; };
//            Conditions[12] = () => { return true; };
//            Conditions[13] = () => { return true; };
//            Conditions[14] = () => { return Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex; };
//            Conditions[15] = () => { return true; };
//            Conditions[16] = () => { return Options.Encoders.AthenaDisplay.Value > 0.8; };
//            Conditions[17] = () => { return true; };
//            Conditions[18] = () => { return true; };
//            Conditions[19] = () => { return Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency1; };
//            Conditions[20] = () => { return true; };
//            Conditions[21] = () => { return true; };
//            Conditions[22] = () => { return true; };
//            Conditions[23] = () => { return Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency4; };
//            Conditions[24] = () => { return Options.Switchers.Power.Value == State.off; };



//            TextLearning = new string[24];

//            TextLearning[0] = "\tНадеть и подогнать шлемофон.";
//            TextLearning[1] = "\tПереключатель рода работ, поставить в положение \"СИМПЛЕКС\".";
//            TextLearning[2] = "\tРучку \"ШУМЫ\" регулятора шумов повернуть против часовой стрелки до упора, т.е. на максимум шумов.";
//            TextLearning[3] = "\tВключить питание радиостанции, при этом загорается лампочка на световом табло \"ПОДДИАПАЗОН\" (\"I\" или \"II\"). Включить освещение шкалы.";
//            TextLearning[4] = "\tПроверить подачу подающих напряжений на приемопередатчик, для чего:";
//            TextLearning[5] = "\tРучку \"ГРОМКОСТЬ\" установить в положение максимальной громкости (вправо до упора). При вращении ручки против часовой стрелки уровень шумов должен меняться до некоторого минимального уровня.";
//            TextLearning[6] = "\tПереключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" установить в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН I\".";
//            TextLearning[7] = "\tВращая ручку \"УСТАНОВКА ЧАСТОТЫ\", прослушать работу приемника по поддиапазону. При исправном приемнике в телефонах шлемофона будет прослушиваться характерный шум или работа других радиостанций.";
//            TextLearning[8] = "\tПроверить работоспособность подавителя шумов: при вращении ручки \"ШУМЫ\" против часовой стрелки уровень шумов должен увеличиваться, а при вращении по часовой стрелке - уменьшаться.";
//            TextLearning[9] = "\tПроверить работу приемника на втором поддиапазоне, установив переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН II\" и повторив операции 8,9.";
//            TextLearning[10] = "\tПереключатель рода работы установить в положение \"ДЕЖ. ПРИЕМ\".";
//            TextLearning[11] = "\tПодвижный визир совместить с неподвижным, повернув винт \"КОРРЕКТОР\". Вращая ручку \"УСТАНОВКА ЧАСТОТЫ\", подвести калибровочную точку, обозначенную на шкале треугольником или прямоугольником, под подвижный визир шкалы.";
//            TextLearning[12] = "\tНажать кнопку \"ТОН-ВЫЗОВ\" и ручкой \"УСТАНОВКА ЧАСТОТЫ\" добиться нулевых биений в телефонах шлемофона. При этом допустимо смещение калибровочной точки от риски визира не более 1/5 делений шкалы между соседними рисками. При больших смещениях необходимо произвести калибровку частоты радиостанции согласно указаниям, приведенным в разделе 17.";
//            TextLearning[13] = "\tПереключатель рода работ установить в положение \"СИМПЛЕКС\".";
//            TextLearning[14] = "\tСпустя 3 минуты (необходимо для прогрева лампы ГУ-50) перевести тангенту нагрудного переключателя в положение \"ПРД\".";
//            TextLearning[15] = "\tНастроить антенную цепь ручкой \"НАСТРОЙКА АНТЕННЫ\" по максимальному отклонению стрелки прибора и максимальному свечению неоновой лампочки на любой частоте поддиапазона.";
//            TextLearning[16] = "\tПроизнести громко \"А\" и прослушать передачу (проверка самопрослушивания) по всему поддиапазону.";
//            TextLearning[17] = "\tНажать кнопку \"ТОН-ВЫЗОВ\" и прослушать сигнал тонального вызова.";
//            TextLearning[18] = "\tПроверить работоспособность передатчика на первом поддиапазоне, установив переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН I\" и повторив операции 16-18.";
//            TextLearning[19] = "\tОткрыть крышку люка на лицевой панели.";
//            TextLearning[20] = "\tУстановить и зафиксировать заданные (или любые) 4 частоты.";
//            TextLearning[21] = "\tНастроить радиостанцию на максимум отдачи на всех 4-х частотах.";
//            TextLearning[22] = "\tУстанавливая переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" поочередно в положение 1, 2, 3, и 4-й фиксированных частот, проверить работу механизма автоматики.\n\tВращение ручек \"УСТАНОВКА ЧАСТОТЫ\" и \"НАСТРОЙКА АНТЕННЫ\" должно происходить без рывков. Не должно наблюдаться сбоев ранее установленных частот по шкале относительно визира, а также расстройки максимума настройки, что наблюдается по уменьшению свечения неоновой лампочки или изменению отклонения стрелки прибора индикатора в положении \"РАБОТА I\".";
//            TextLearning[23] = "\tВыключить радиостанцию.";
//        }
//    }
//}
