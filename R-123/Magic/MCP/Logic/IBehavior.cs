namespace MCP.Logic
{
    interface IBehavior
    {
        AudioPlayerState GetState(byte[] information);
    }
}
