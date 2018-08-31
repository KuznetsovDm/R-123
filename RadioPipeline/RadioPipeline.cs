using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadioPipeline
{
    //Base what i want
    //player <-- mixer <-- global noize filter <-- local noize filter <-- playing filter <-- audio codec <-- data parsing <-- data avaliable
    //audio data avaliable --> audio codec --> data code --> send data

    //Protorype - relative version
    public class RadioPipeline
    {
        //IRadioParameterInterface
        //like a frequency 
        public object RadioParameters { get; set; }

        //AudioMixHandler
        public object AudioMixer { get; set; }

        //Noise handler
        public object NoiseHandler { get; set; }
    }

    public class RadioContex
    {
        public byte[] RawData { get; set; }
    }

}
