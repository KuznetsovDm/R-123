using RadioTask.Model.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioTask.Model.RadioContexts
{
    public abstract class RadioContext
    {
        protected R123.Radio.Model.MainModel Radio { get; set; }
        public bool IsSubscribe { get; protected set; }
        //подписывается на радиостанцию
        public abstract void Subscribe();
        //отписывается от радиостанции
        public abstract void Unsubscribe();
        //возвращает состояние проверки
        public abstract bool GetState();
        //когда происходит событие вызвать событие обработчик
        public virtual void WhenActivateToInvoke(Action action)
        {
            this.action = action;
        }
        //свойство которое будем передавать
        protected Action action { get; set; }
        public abstract TypeRadioAction GetActionType();
    }
}
