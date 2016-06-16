using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HotChocolatey.Utility;

namespace HotChocolatey.Model.ChocoTask
{
    internal abstract class BaseChocoTask
    {
        public async Task Execute()
        {
            bool result = await Execute($"{GetCommand()} --limitoutput {GetParameters()}", GetOutputLineCallback());
            AfterExecute(result);
        }

        protected abstract string GetCommand();
        protected abstract string GetParameters();
        protected abstract Action<string> GetOutputLineCallback();

        private async Task<bool> Execute(string arguments, Action<string> outputLineCallback)
        {
            Log.Info($">> choco {arguments}");
            ChocoCommunication.Log($"choco {arguments}", CommunicationDirection.ToChoco);

            using (Process choco = new Process())
            {
                choco.StartInfo.FileName = "choco";
                choco.StartInfo.Arguments = arguments;
                choco.StartInfo.UseShellExecute = false;
                choco.StartInfo.RedirectStandardOutput = true;
                choco.StartInfo.CreateNoWindow = true;
                choco.Start();

                bool endOfStream = false;
                while (!endOfStream)
                {
                    string line = await choco.StandardOutput.ReadLineAsync();
                    Log.Info(line);
                    ChocoCommunication.Log(line, CommunicationDirection.FromChoco);
                    outputLineCallback(line);

                    await Task.Run(() => endOfStream = choco.StandardOutput.EndOfStream);
                }

                if (choco.ExitCode != 0)
                {
                    Log.Error($">> choco {arguments} exited with code {choco.ExitCode}");
                }

                ChocoCommunication.Log($"exited with code {choco.ExitCode}", CommunicationDirection.FromChoco);

                return choco.ExitCode == 0;
            }
        }

        protected virtual void AfterExecute(bool result)
        {
            // 
        }
    }
}