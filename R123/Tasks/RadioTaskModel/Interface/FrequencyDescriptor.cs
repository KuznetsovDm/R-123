using RadioTask.Model.RadioContexts.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R123.Tasks.RadioTaskModel.Interface
{
    public class FrequencyDescriptor
    {
        public FrequencyParameter Parameter { get; set; }

        public override string ToString()
        {
            string result = "";
            if (Parameter != null)
            {
                result += Parameter.Frequency + " МГц.";
            }
            return result;
        }
    }
}
