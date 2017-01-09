using UnityEngine;

namespace Ts.Unity
{
    public class UnityLogger : Logger
    {
        private readonly string _name;

        public UnityLogger(string name)
        {
            _name = name;
        }

        string Logger.Name{
            get {
                return _name;
            }
        }

        void Logger.Debug(string fmt, params object[] args)
        {
            Debug.Log(string.Format(_name + ": " + fmt, args));
        }

        void Logger.Info(string fmt, params object[] args)
        {
            Debug.Log(string.Format(_name + ": " + fmt, args));
        }

        void Logger.Warn(string fmt, params object[] args)
        {
            Debug.LogWarning(string.Format(_name + ": " + fmt, args));
        }

        void Logger.Error(string fmt, params object[] args)
        {
            Debug.LogError(string.Format(_name + ": " + fmt, args));
        }
    }
}
