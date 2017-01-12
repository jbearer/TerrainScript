namespace Ts.Unity
{
    public class UnityLogger : Logging.Logger
    {
        private readonly string _name;

        public UnityLogger(string name)
        {
            _name = name;
        }

        public override string Name{
            get {
                return _name;
            }
        }

        protected override void Log(Logging.Level level, string fmt, params object[] args)
        {
            if (level < Logging.Level.Warning) {
                UnityEngine.Debug.Log(string.Format(_name + ": " + fmt, args));
            } else if (level < Logging.Level.Error) {
                UnityEngine.Debug.LogWarning(string.Format(_name + ": " + fmt, args));
            } else {
                UnityEngine.Debug.LogError(string.Format(_name + ": " + fmt, args));
            }
        }
    }
}
