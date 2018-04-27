using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using R123;
using R123.Connection.Audio;

namespace SignalRBase.Proxy
{
    public class RadioProxy : RadioProxyLogic, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RadioProxy(IPAddress ip, int port, string hubName = "RadioHub")
        {
            Ip = ip;
            Port = port;
            HubName = hubName;
            Connection = new HubConnection(ConnectionInfo.GetUrlConnection(ip, port));
            RemoteRadio = Connection.CreateHubProxy(HubName);
            RemoteRadio.On("RemoteClose", () => OnRemoteClosed(new ParameterEventArgs<RadioProxy>(this)));
            RemoteRadio.On<double>("Frequency", SetFrequency);
            RemoteRadio.On<bool>("PlayTone", PlayTone);
            RemoteRadio.On<bool>("Saying", Saying);
            Audio = new RadioProxyAudio();
            Connection.StateChanged += (x) => ConnectionState(x.NewState);
        }

        public event EventHandler<ParameterEventArgs<RadioProxy>> RemoteClosed;

        public Task Start()
        {
            return Connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    foreach (var exception in task.Exception.InnerExceptions)
                    {
                        Logger.Log(exception);
                    }
                }
                else
                {
                    RemoteRadio.Invoke("Subscribe").ContinueWith(x =>
                    {
                        Logger.Log($"Subscribed.");
                        RemoteRadio.Invoke<string>("StreamingInfo").ContinueWith(y =>
                        {
                            var info = ConnectionInfo.Parse(y.Result);
                            Audio.InitListener(new AudioListener(info.Address, info.Port));
                            Logger.Log($"Connected to:{Ip}:{Port} - {info.ToString()}");
                        });
                    });
                }
            });
        }

        public Task Stop()
        {
            try
            {
                return RemoteRadio.Invoke("Unsubscribe").ContinueWith(x =>
                {
                    Connection.Stop();
                });
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }

            return null;
        }

        public IHubProxy RemoteRadio { get; private set; }

        public string HubName { get; private set; }

        public HubConnection Connection { get; private set; }

        private IPAddress ip;
        public IPAddress Ip
        {
            get => ip;
            set
            {
                ip = value;
                OnPropertyChanged("Ip");
            }
        }

        public new void SetFrequency(double frequency)
        {
            base.SetFrequency(frequency);
            OnPropertyChanged("Frequency");
        }

        public int Port { get; private set; }

        public RadioProxyAudio Audio { get; private set; }

        protected virtual void OnRemoteClosed(ParameterEventArgs<RadioProxy> e)
        {
            SetFrequency(0);
            Saying(false);
            PlayTone(false);
            RemoteClosed?.Invoke(this, e);
        }
        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}