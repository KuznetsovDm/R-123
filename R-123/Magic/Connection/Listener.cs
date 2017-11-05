using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Connection
{
    public class Listener : IListener
    {
        public bool IsListening { get; private set; } = false;

        Connection connection;

        public EventHandler<MessageAvailableEventArgs> MessageAvailableEvent;
        EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        Thread listeningThread;

        public void Close()
        {
            MessageAvailableEvent = null;
            IsListening = false;
            waitHandle.Reset();
            listeningThread.Abort();
            connection.Close();
        }

        public void Init(Connection connection)
        {
            this.connection = connection;
            if (!IsOpen)
                connection.Open();
            listeningThread = new Thread(Listen);
            listeningThread.Start();
        }

        public void Start()
        {
            if (!IsListening)
                IsListening = true;
            waitHandle.Set();
        }

        public void Stop()
        {
            if (IsListening)
                IsListening = false;
            waitHandle.Reset();
        }

        void Listen()
        {
            while (true)
            {
                while (IsListening)
                {
                    Task<byte[]> task = connection.Receive();
                    byte[] message = task.Result;
                    MessageAvailableEventArgs e = new MessageAvailableEventArgs(message);
                    MessageAvailableEvent?.Invoke(this, e);
                }
                waitHandle.WaitOne();
            }
        }

        public bool IsOpen
        {
            get
            { return connection.IsOpen; }
            private set
            { /*nothing to do*/ }
        }
    }
}
