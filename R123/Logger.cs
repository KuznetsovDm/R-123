using System;
using System.IO;
using System.Threading;

namespace R123
{
    public class Logger
    {
        private static string logpath;
        static object lockObject;
        static Logger()
        {
            lockObject = new object();
            logpath = "applog.log";
        }

        public static void Log(Exception exception)
        {
            Log(exception.Message);
        }

        public static void Log(string info)
        {
            Monitor.Enter(lockObject);
            File.AppendAllText(logpath, info);
            Monitor.Exit(lockObject);
        }
    }
}