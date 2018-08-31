using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using P2PMulticastNetwork;
using P2PMulticastNetwork.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace AudioClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = "../../../../music/1.mp3";
            if(args.Length > 0)
                path = args[0];

            var options = new MulticastConnectionOptions
            {
                ExclusiveAddressUse = false,
                UseBind = false,
            };

            var guid = Guid.NewGuid();
            var converter = new DataModelConverter();

            var transmitter = new UdpMulticastConnection(options);
            using(var audioFile = new AudioFileReader(path))
            {
                var buffer = new byte[audioFile.WaveFormat.SampleRate];
                var bufferProvider = new BufferedWaveProvider(audioFile.WaveFormat);
                double d = audioFile.WaveFormat.AverageBytesPerSecond / audioFile.WaveFormat.SampleRate;
                var timer = new Timer((x)=>
                {
                    audioFile.Read(buffer, 0, buffer.Length);
                    var data = new DataModel
                    {
                        Guid = guid,
                        RawAudioSample = buffer
                    };

                    transmitter.Write(converter.ConvertToBytes(data));
                });

                timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(1000 / d));
                while(true)
                    Thread.Sleep(1000);
                timer.Dispose();
            }
            Console.ReadKey();
        }
    }
}
