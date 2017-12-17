using System;
using HotChocolatey.Utility;

namespace HotChocolatey.Model.ChocoTask
{
    public class ChocoCommunication : chocolatey.infrastructure.logging.ILog
    {
        private readonly Action<string> outputLineCallback;

        public ChocoCommunication(Action<string> outputLineCallback)
        {
            this.outputLineCallback = outputLineCallback;
        }

        public bool IsSuccess { get; private set; } = true;

        public void InitializeFor(string loggerName)
        { }

        public void Debug(string message, params object[] formatting)
        { }

        public void Debug(Func<string> message)
        { }

        public void Info(string message, params object[] formatting)
        {
            OutputAndLog(message, formatting);
        }

        public void Info(Func<string> message)
        {
            OutputAndLog(message());
        }

        public void Warn(string message, params object[] formatting)
        {
            OutputAndLog(message, formatting);
        }

        public void Warn(Func<string> message)
        {
            OutputAndLog(message());
        }

        public void Error(string message, params object[] formatting)
        {
            OutputAndLog(message, formatting);
            IsSuccess = false;
        }

        public void Error(Func<string> message)
        {
            OutputAndLog(message());
            IsSuccess = false;
        }

        public void Fatal(string message, params object[] formatting)
        {
            Log.Error($"Choco Fatal: ${string.Format(message, formatting)}");
            IsSuccess = false;
        }

        public void Fatal(Func<string> message)
        {
            Log.Error($"Choco Fatal: ${message()}");
            IsSuccess = false;
        }

        private void OutputAndLog(string message, object[] formatting)
        {
            OutputAndLog(string.Format(message, formatting));
        }

        private void OutputAndLog(string line)
        {
            Log.Info(line);
            outputLineCallback(line);
        }
    }
}