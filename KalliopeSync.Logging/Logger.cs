using System;
using log4net;
using log4net.Config;

namespace KalliopeSync.Logging
{
    public static class Logger
    {
        private static readonly ILog L4NLogger = log4net.LogManager.GetLogger(typeof(KalliopeSync.Logging.Logger));

        public static void Init()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void Info(string message, Exception ex= null)
        {
            L4NLogger.Info(message, ex);
        }

        public static void Debug(string message, Exception ex= null)
        {
            L4NLogger.Debug(message, ex);
        }

        public static void Warn(string message, Exception ex= null)
        {
            L4NLogger.Warn(message, ex);
        }

        public static void Error(string message, Exception ex= null)
        {
            L4NLogger.Error(message, ex);
        }
    }
}

