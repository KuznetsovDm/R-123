using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RadioTask.Model.Chain
{
    public class Handler : IObserver<Step>, IDisposable
    {
        public List<Step> Steps { get; internal set; }
        public Step PriorityStep { get;private set; }
        public event EventHandler<ErrorEventArgs> Error = delegate { };
        public event EventHandler AllStepsPassed = delegate { };
        public bool IsWork { get; protected set; } = false;
        public bool MustICheckSequency { get; set; }

        public Handler()
        {
            Steps = new List<Step>();
        }

        public virtual void Handle(Step obj)
        {
            if (MustICheckSequency)
            {
                //если была ошибка дальше не обрабатывать
                if (HandleError(obj))
                    return;
                PrepareForNextStep(obj);
                HandlePassedState(obj);
            }
            else
            {
                if (Steps.All(x => x.CurrentState))
                {
                    AllStepsPassed(this, new EventArgs());
                }
            }
        }

        //возвращает информацию была ли ошибка
        private bool HandleError(Step current)
        {
            if (current != PriorityStep)
            {
                if (PriorityStep.ErrorHandler.ErrorRelation.Contains(current.Type))
                {
                    return false;
                }
                else
                {
                    if (PriorityStep.ErrorHandler.SkipTypesNext.Contains(current.Type))
                    {
                        return false;
                    }
                    PriorityStep.ErrorHandler.ErrorRelation.Add(current.Type);
                    current.ErrorHandler.ErrorRelation.Add(PriorityStep.Type);

                    ErrorEventArgs args = new ErrorEventArgs() { ForStep = PriorityStep, WrongSteps = new List<Step>() { current } };
                    Error(this, args);
                    return true;
                }
            }
            return false;
        }

        private void PrepareForNextStep(Step previous)
        {
            //выключаем обработку предыдущего шага, и в ключаем обработку следущего. Если это возможно.
            if (PriorityStep.CurrentState)
            {
                int index = Steps.FindIndex(x => x == PriorityStep);
                if (Steps.Count > index && index >= 0 && index != Steps.Count - 1) 
                {
                    while (index < Steps.Count - 1 && PriorityStep.CurrentState)
                    {
                        PriorityStep = Steps[index + 1];
                        index++;
                    }
                }
            }

        }

        private void HandlePassedState(Step current)
        {
            if (CheckWithoutEscapedPrew(Steps.Last(), Steps))
            {
                Steps.ForEach(x =>
                {
                    if (x.IsOn)
                        x.TurnOff();
                    x.RemoveObserver();
                });
                PriorityStep = null;
                AllStepsPassed(this, new EventArgs());
            }
        }

        private bool CheckWithoutEscapedPrew(Step interested, List<Step> steps)
        {
            var stepsWithoutEscaped = (from item in steps
                                      where !interested.ErrorHandler.SkipTypesPrew.Contains(item.Type)
                                      select item).ToList();
            return stepsWithoutEscaped.All(x => x.CurrentState);
        }

        public virtual void Start()
        {
            if (Steps.Count < 1)
                throw new Exception("Are't steps.");
            Steps.ForEach(
                x => {
                    x.AddObserver(this);
                    if (!x.IsOn)
                        x.TurnOn();
                });
            PriorityStep = Steps.First();
            //включаем на прослушивание первый шаг
            IsWork = true;
        }

        public virtual void Stop()
        {
            Steps.ForEach(x =>
            {
                x.RemoveObserver();
                if (x.IsOn)
                    x.TurnOff();
            }
            );
            IsWork = false;
        }

        public virtual void Dispose()
        {
            if (IsWork)
                Stop();
            Steps.Clear();
            Steps = null;
        }

        protected virtual void OnError(ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        protected virtual void OnAllStepsPassed(EventArgs e)
        {
            AllStepsPassed?.Invoke(this, e);
        }
    }
}
