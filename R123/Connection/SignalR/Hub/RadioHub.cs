using System.IO.Ports;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using Microsoft.AspNet.SignalR.Client.Hubs;
using R123;
using R123.Connection.Logic;
using R123.StartTab;
using SignalR;
using SignalR.Hubs;

namespace SignalRBase.Hub
{
    [HubName("RadioHub")]
    public class RadioHub : SignalR.Hubs.Hub
    {
        private object lockObject = new object();
        public static RadioLogic Logic { get; set; }

        public void Subscribe()
        {
            Groups.Add(Context.ConnectionId, GroupString);
            Logger.Log(Context.ConnectionId);
            if (Logic != null)
            {
                Frequency(Logic.Frequency);
                Saying(Logic.SayingState);
            }
        }

        public void Unsubscribe()
        {
            Groups.Remove(Context.ConnectionId, GroupString);
        }

        public string StreamingInfo()
        {
            return ConnectionInfo.StreamAddressInfo.ToString();
        }

        public static IHubContext HubContext
        {
            get => GlobalHost.ConnectionManager.GetHubContext<RadioHub>();
        }

        public static string GroupString
        {
            get => "RadioConnection";
        }

        public static void Frequency(double frequency)
        {
            HubContext.Clients[GroupString].Frequency(frequency);
        }

        public static void PlayTone(bool state)
        {
            HubContext.Clients[GroupString].PlayTone(state);
        }

        public static void Saying(bool state)
        {
            HubContext.Clients[GroupString].Saying(state);
        }

        public static void Close()
        {
            HubContext.Clients[GroupString].RemoteClose();
        }
    }
}