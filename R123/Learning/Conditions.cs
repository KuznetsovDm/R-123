using System;
using System.Collections.Generic;

namespace R123.Learning
{
    public class Conditions
    {
        private List<Func<bool>> conditions;

        public Conditions()
        {
            conditions = new List<Func<bool>>();
        }

        public Conditions Add(Func<bool> condition)
        {
            conditions.Add(condition);
            return this;
        }

        public bool CheckConditionByIndex(int index)
        {
            return conditions[index]();
        }

        public int Length { get { return conditions.Count; } }
    }
}
