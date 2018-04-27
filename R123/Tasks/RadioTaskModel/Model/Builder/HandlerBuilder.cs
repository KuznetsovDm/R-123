using R123.Radio.Model;
using RadioTask.Model.Builder.BuilderConfiguration;
using RadioTask.Model.Chain;

namespace RadioTask.Model.Builder
{
    public class HandlerBuilder
    {
        HandlerConfiguration configuration;
        public HandlerBuilder(MainModel radio)
        {
            Handler = new Handler();
            configuration = new HandlerConfiguration(Handler, radio);
        }

        public HandlerConfiguration BuildStep()
        {
            return configuration;            
        }

        public Handler Handler { get; private set; }
    }
}
