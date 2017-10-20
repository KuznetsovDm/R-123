using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioLogic
{
    interface IConnectionProvider
    {
        void Start(decimal frequency);

        void Stop();

        void SetFrequency(decimal frequency);

        void SetNoise(double noise);

        void StartStreaming();

        void StopStreaming();

        void Close();
    }
}
