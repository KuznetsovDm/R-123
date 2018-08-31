using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CSharpFunctionalExtensions;
using System.Diagnostics;

namespace P2PMulticastNetwork
{
    public class UdpMulticastConnection : IDataReceiver, IDataTransmitter
    {
        private IPEndPoint _sendTo;
        private MulticastConnectionOptions _options;
        private UdpClient _client;
        private bool _wasClose;
        private CancellationTokenSource  _cancellToken;

        public UdpMulticastConnection(MulticastConnectionOptions options)
        {
            _sendTo = new IPEndPoint(options.MulticastAddress, options.Port);
            _options = options;
            _client = new UdpClient();
            _client.ExclusiveAddressUse = options.ExclusiveAddressUse;
            _cancellToken = new CancellationTokenSource();
            if(options.UseBind)
                _client.Client.Bind(new IPEndPoint(IPAddress.Any, options.Port));
            _client.JoinMulticastGroup(options.MulticastAddress);
            _client.MulticastLoopback = options.MulticastLoopback;
        }

        public void Dispose()
        {
            Result result = SafeClose();
            if(result.IsFailure)
                Debug.WriteLine($"Error in {nameof(UdpMulticastConnection)}: {result.Error}");
        }

        private Result SafeClose()
        {
            if(!_wasClose)
            {
                try
                {
                    _cancellToken.Cancel();
                    _client.DropMulticastGroup(_options.MulticastAddress);
                    _client.Close();
                }
                catch(Exception ex)
                {
                    Result.Fail(ex.ToString());
                }
                _wasClose = true;
            }

            return Result.Ok();
        }

        public async Task<Result<byte[]>> Receive()
        {
            return await Task.Factory.StartNew<Result<byte[]>>(() =>
            {
                try
                {
                    IPEndPoint ep = null;
                    byte[] data = _client.Receive(ref ep);
                    return Result.Ok(data);
                }
                catch(Exception ex)
                {
                    return Result.Fail<byte[]>(ex.ToString());
                }
            },
            _cancellToken.Token);
        }

        public Result Write(byte[] data)
        {
            try
            {
                _client.Send(data, data.Length, _sendTo);
                return Result.Ok();
            }
            catch(Exception ex)
            {
                return Result.Fail(ex.ToString());
            }
        }
    }
}
