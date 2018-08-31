using System.Configuration;

namespace P2PMulticastNetwork
{
    public interface IConfiguration
    {
        string GetInfo(string name);
    }

    public class DefaultAppConfiguration : IConfiguration
    {
        public string GetInfo(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}