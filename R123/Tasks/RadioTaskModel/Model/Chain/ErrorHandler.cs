using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioTask.Model.Chain
{
    public class ErrorHandler
    {
        public List<TypeRadioAction> SkipTypesPrew { get; set; } = new List<TypeRadioAction>();

        public List<TypeRadioAction> SkipTypesNext { get; set; } = new List<TypeRadioAction>();

        public HashSet<TypeRadioAction> ErrorRelation { get; set; } = new HashSet<TypeRadioAction>();

        public virtual bool HandleNext(Step priorityStep, Step handled,List<Step> Steps)
        {
            if (SkipTypesNext.Contains(handled.Type))
                return false;
            else
            {
                return Steps.IndexOf(handled) > Steps.IndexOf(priorityStep);
            }

        }

        public virtual List<Step> HandlePrew(Step forStep, List<Step> Steps)
        {
            var previousSteps = Steps.TakeWhile((x) => x != forStep);
            var query = from item in previousSteps
                        where !SkipTypesPrew.Contains(item.Type)
                        select item;
            var haveError = query.Where(x=>!x.CurrentState);
            return haveError.ToList();
        }
    }
}
