using System;
using System.ServiceProcess;
using Process = System.Diagnostics.Process;

namespace BuildHelper
{
    internal static class WinHelper
    {
        /// <summary>
        /// Starts service if it's stopped. Returns True if service was started.
        /// </summary>
        public static bool StartServiceIfNeeded(string serviceName, int timeout = 30)
        {
            //Guard.Valid(serviceName, nameof(serviceName));
            bool running = ServiceIsRunning(serviceName);
            if (running)
                return false;

            using (ServiceController sc = new ServiceController(serviceName))
            {
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    //m_Logger.WriteLine($"Starting {serviceName}");
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                }
                if (sc.Status == ServiceControllerStatus.StartPending)
                {
                    //m_Logger.WriteLine($"Starting {serviceName}");
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(timeout));
                }
            }

            //m_Logger.WriteLine($"Service {serviceName} has started.");
            return true;
        }

        /// <summary>
        /// Stops service if it's running. Returns True if service was stopped.
        /// </summary>
        public static bool StopServiceIfNeeded(string serviceName, int timeout = 30)
        {
            //Guard.Valid(serviceName, nameof(serviceName));
            bool running = ServiceIsRunning(serviceName);
            if (!running)
                return false;

            using (ServiceController sc = new ServiceController(serviceName))
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    //m_Logger.WriteLine($"Stopping {serviceName}");
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                if (sc.Status == ServiceControllerStatus.StopPending)
                {
                    //m_Logger.WriteLine($"Stopping {serviceName}");
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(timeout));
                }
            }

            //m_Logger.WriteLine($"Service {serviceName} has stoped.");
            return true;
        }

        public static bool StartProcessIfNeeded(string name, int timeout = 30)
        {
            var runningProcesses = Process.GetProcessesByName(name);
            if (runningProcesses.Length == 0)
            {
                try
                {
                    var process = Process.Start(name);
                    return true;
                }
                catch (Exception ex)
                { }
            }

            return false;
        }

        public static bool StopProcessIfNeeded(string name, int timeout = 30)
        {
            var runningProcesses = Process.GetProcessesByName(name);
            if (runningProcesses.Length == 0)
                return false;

            foreach (var process in runningProcesses)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                { }
            }

            return true;
        }

        private static bool ServiceIsRunning(string serviceName)
        {
            using (ServiceController sc = new ServiceController(serviceName))
                if (sc.Status == ServiceControllerStatus.Stopped)
                    return false;

            return true;
        }
    }
}
