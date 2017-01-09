using System;
using System.Collections.Generic;

namespace Ts
{
    public interface Logger
    {
        string Name{ get; }

        void Debug(string fmt, params object[] args);
        void Info(string fmt, params object[] args);
        void Warn(string fmt, params object[] args);
        void Error(string fmt, params object[] args);
    }

    public static class Logging
    {
        private static Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();

        public static void SetImpl(Func<string, Logger> factory)
        {
            newLogger = factory;
        }

        public static Logger GetLogger(string name)
        {
            if (!_loggers.ContainsKey(name)) _loggers.Add(name, newLogger(name));
            return _loggers[name];
        }

        // Create UnityLogger_s by default
        private static Func<string, Logger> newLogger = name => new Unity.UnityLogger(name);
    }
}
