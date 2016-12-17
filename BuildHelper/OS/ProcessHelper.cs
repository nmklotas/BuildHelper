using BuildHelper.Log;
using EnsureThat;
using System;
using System.Diagnostics;
using System.IO;

namespace BuildHelper.OS
{
    internal class ProcessHelper
    {
        private readonly IExtensionLogger m_ExtensionLogger;

        public ProcessHelper(IExtensionLogger extensionLogger)
        {
            Ensure.That(() => extensionLogger).IsNotNull();
            m_ExtensionLogger = extensionLogger;
        }

        public bool StartProcessIfNeeded(string processPath, int timeout = 30)
        {
            Ensure.That(() => processPath).IsNotNullOrEmpty();
            string name = Path.GetFileNameWithoutExtension(processPath);

            var runningProcesses = Process.GetProcessesByName(name);
            if (runningProcesses.Length != 0)
                return false;

            try
            {
                WriteStatus($"Starting process {name} ...");
                Process.Start(processPath);
                return true;
            }
            catch (Exception ex)
            {
                WriteStatus($"Failed to start process {name} more information exists in the activity log.");
                WriteException(ex);
            }

            return false;
        }

        public bool StopProcessIfNeeded(string processPath, int timeout = 30)
        {
            Ensure.That(() => processPath).IsNotNullOrEmpty();
            string name = Path.GetFileNameWithoutExtension(processPath);

            var runningProcesses = Process.GetProcessesByName(name);
            if (runningProcesses.Length == 0)
                return false;

            foreach (var process in runningProcesses)
            {
                try
                {
                    WriteStatus($"Killing process {name} ...");
                    process.Kill();
                }
                catch (Exception ex)
                {
                    WriteStatus($"Failed to kill process {name} more information exists in the activity log.");
                    WriteException(ex);
                }
            }

            return true;
        }

        private void WriteStatus(string message)
        {
            m_ExtensionLogger.WriteStatus(message);
        }

        private void WriteException(Exception exception)
        {
            m_ExtensionLogger.WriteException(exception);
        }
    }
}
