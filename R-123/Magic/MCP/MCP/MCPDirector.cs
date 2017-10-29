using MCP.Audio;
using MCP.Logic;

namespace MCP
{
    class MCPDirector
    {
        MCPConnector connector;
        IBehavior behavior;
        MCPStorage storage;
        AudioManager audioManager;

        public MCPDirector(MCPConnector connector, IBehavior behavior,AudioManager audioManager)
        {
            this.audioManager = audioManager;
            this.connector = connector;
            this.behavior = behavior;
            storage = new MCPStorage();
            connector.NewMCPPacket += NewMCPPacket;
        }

        private void NewMCPPacket(object sender, MCPConnector.MCPEventArgs e)
        {
            MCPPacket packet = e.packet;
            ComputeState(packet);
        }

        /// <summary>
        /// Начинает работу по отправке сообщений на multicast адрес.
        /// </summary>
        public void Start()
        {
            connector.Start();
            SetStateForAllPackets();
        }

        /// <summary>
        /// Останавливает посылку пакетов и прослушивание всех плееров.
        /// </summary>
        public void Stop()
        {
            connector.Stop();
            foreach (var packet in storage.GetPackets())
                if (packet.State == MCPState.Information)
                {
                    var state = behavior.GetState(packet.Information);
                    state.Play = false;
                    audioManager.Operator(packet, state);
                }
        } 

        /// <summary>
        /// Определяет состояние пакета и передает управляющий сигнал плееру, созданного для пакета того-же типа(IP и Port).
        /// </summary>
        /// <param name="packet"></param>
        private void ComputeState(MCPPacket packet)
        {
            AudioPlayerState state;
            bool contains = storage.ContainsValue(packet);
            bool isNewMessage = storage.IsNewMessage(packet);
            storage.AddValueOrUpdate(packet);
            if (packet.State == MCPState.Close)
            {
                storage.Remove(packet);
                //will close and delete NetAudioPlayer
                state = new AudioPlayerState(false, 0, true);
                audioManager.Operator(packet, state);
            }
            else if(isNewMessage)
            {
                if (packet.State == MCPState.Information)
                {
                    state = behavior.GetState(packet.Information);
                    if (!contains)
                        state.Create = true;
                }
                else
                {
                    state = (contains)?new AudioPlayerState():new AudioPlayerState(false,0,false,create: true);
                }
                audioManager.Operator(packet, state);
            }
        }

        /// <summary>
        /// Производится пересчет состояний для всех плееров, которых мы слушаем.
        /// </summary>
        public void SetStateForAllPackets()
        {
            foreach (var packet in storage.GetPackets())
                if (packet.State == MCPState.Information)
                    audioManager.Operator(packet, behavior.GetState(packet.Information));
        }

        /// <summary>
        /// Отсылаем байты в по multicast адресу.
        /// </summary>
        /// <param name="information"></param>
        public void Send(byte[] information)
        {
            connector.Send(information);
        }

        /// <summary>
        /// Освобождает ресурсы и останавливает всю работу класса.
        /// </summary>
        public void CLose()
        {
            //Задаем всем плеерам состояние для закрытие плеера
            foreach (var element in storage.GetPackets())
                audioManager.Operator(element, new AudioPlayerState(false, 0, true));
            connector.Close();
            audioManager.Close();
            storage = null;
        }
    }
}
