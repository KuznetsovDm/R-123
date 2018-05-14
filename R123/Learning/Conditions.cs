using System;
using System.Collections;
using System.Collections.Generic;

namespace R123.Learning
{
    public class Conditions : IEnumerable<Func<bool>>
    {
        private readonly List<Func<bool>> conditions;

        public Conditions()
        {
            conditions = new List<Func<bool>>();
        }

        public int Length => conditions.Count;

        public Conditions Add(Func<bool> condition)
        {
            conditions.Add(condition);
            return this;
        }

        public bool CheckConditionByIndex(int index)
        {
            return conditions[index]();
        }

        public IEnumerator<Func<bool>> GetEnumerator()
        {
            return conditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return conditions.GetEnumerator();
        }
    }
}