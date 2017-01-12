using System;
using System.Collections.Generic;
using System.IO;

namespace Ts
{
    public static class Logging
    {
        public enum Level
        {
            Debug = 1,
            Info = 2,
            Warning = 3,
            Error = 4
        }

        public abstract class Logger
        {
            public abstract string Name{ get; }

            public void Debug(string fmt, params object[] args)
            {
                Log(Level.Debug, fmt, args);
            }
            public void Info(string fmt, params object[] args)
            {
                Log(Level.Info, fmt, args);
            }
            public void Warn(string fmt, params object[] args)
            {
                Log(Level.Warning, fmt, args);
            }
            public void Error(string fmt, params object[] args)
            {
                Log(Level.Error, fmt, args);
            }

            protected abstract void Log(Level level, string fmt, params object[] args);
        }

        public abstract class StaticLogger<ImplType>
            where ImplType : StaticLogger<ImplType>, new()
        {
            private static ImplType _impl = new ImplType();

            protected abstract Logger log { get; }

            public static void Debug(string fmt, params object[] args)
            {
                _impl.log.Debug(fmt, args);
            }

            public static void Info(string fmt, params object[] args)
            {
                _impl.log.Info(fmt, args);
            }

            public static void Warn(string fmt, params object[] args)
            {
                _impl.log.Warn(fmt, args);
            }

            public static void Error(string fmt, params object[] args)
            {
                _impl.log.Error(fmt, args);
            }
        }

        public class FileLogger : Logger
        {
            private static StreamWriter _file = new StreamWriter(
                File.Open("terrain-script.log", FileMode.Append, FileAccess.Write));

            private readonly string _name;
            private Level _flushOn;

            public override string Name
            {
                 get {
                    return _name;
                 }
            }

            public FileLogger(string name, Level flushOn = Level.Warning)
            {
                _name = name;
                _flushOn = flushOn;
            }

            protected override void Log(Level level, string fmt, params object[] args)
            {
                string levelString = "";
                switch (level) {
                case Level.Debug:
                    levelString = "D ";
                    break;
                case Level.Info:
                    levelString = "I ";
                    break;
                case Level.Warning:
                    levelString = "W ";
                    break;
                case Level.Error:
                    levelString = "E ";
                    break;
                }

                _file.Write(DateTime.Now.ToString("HH:mm:ss.fff ") +
                    levelString + _name + " " + fmt + "\n", args);

                if (level >= _flushOn) {
                    _file.Flush();
                }
            }
        }

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

        // Create FileLogger_s by default
        private static Func<string, Logger> newLogger = name => new FileLogger(name, Level.Debug);
    }

    // Sometimes it's convenient to output a log method during debugging without creating a new logger
    public class Log : Logging.StaticLogger<Log>
    {
        private static Logging.Logger _log = Logging.GetLogger("global");
        protected override Logging.Logger log
        {
            get {
                return Log._log;
            }
        }
    }
}
