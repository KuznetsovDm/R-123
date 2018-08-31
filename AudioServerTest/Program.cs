using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using P2PMulticastNetwork;
using P2PMulticastNetwork.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AudioServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IDataMiner miner1 = new DataEngineMiner();
            var wformat = WaveFormat.CreateIeeeFloatWaveFormat(44100,2);
            var converter = new DataModelConverter();

            var dict = new ConcurrentDictionary<Guid,BufferedWaveProvider>();
            MixingSampleProvider mixer = new MixingSampleProvider(wformat);
            mixer.ReadFully = true;
            miner1.OnDataAwaliable((data) =>
            {
                DataModel obj = converter.ConvertFrom(data);

                var buffer = dict.GetOrAdd(obj.Guid, (guid)=>
                {
                    var buf = new BufferedWaveProvider(wformat);
                    mixer.AddMixerInput(buf);
                    return buf;
                });
                buffer.AddSamples(obj.RawAudioSample, 0, obj.RawAudioSample.Length);
            });
            MulticastConnectionOptions options = new MulticastConnectionOptions
            {
                ExclusiveAddressUse = false,
                MulticastLoopback = true,
                UseBind = true,
            };
            var reader = new UdpMulticastConnection(options);
            miner1.ReloadDataReceiver(reader);

            var waveOut = new WaveOutEvent();
            waveOut.Init(mixer);
            miner1.Start();
            waveOut.Play();
            Console.ReadKey();
            miner1.Stop();
            waveOut.Dispose();
        }
    }
}
