using RadioTask.Model.RadioContexts.Info;

namespace R123.Tasks.RadioTaskModel.Interface
{
    public class FixFrequencyDescriptor
    {
        public FixFrequencyParameter Parameter { get; set; }

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
