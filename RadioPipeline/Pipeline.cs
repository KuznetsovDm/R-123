using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioPipeline
{
    public delegate void PipelineDelegate<T>(T context);

    public class PipelineBuilder<T>
    {
        private LinkedList<Action<T, PipelineDelegate<T>>> _actions;

        public PipelineBuilder()
        {
            _actions = new LinkedList<Action<T, PipelineDelegate<T>>>();            
        }

        public PipelineDelegate<T> Build()
        {
            PipelineDelegate<T> currentDelegate = delegate { };
            foreach (var action in _actions)
            {
                var prevDelegate = currentDelegate;
                currentDelegate = context =>
                {
                    action(context,prevDelegate);
                };
            }

            return currentDelegate;
        }

        public PipelineBuilder<T> Use(Action<T,PipelineDelegate<T>> action)
        {
            _actions.AddFirst(action);
            return this;
        }
    }

    //player <-- mixer <-- global noize filter <-- local noize filter <-- playing filter <-- audio codec <-- data parsing <-- data avaliable
    //audio data avaliable --> audio codec --> data code --> send data
}
