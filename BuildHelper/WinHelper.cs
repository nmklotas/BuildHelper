using EnsureThat;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Linq;
using System.ServiceProcess;
using Process = System.Diagnostics.Process;

namespace BuildHelper
{
    internal class WinHelper
    {
        private readonly IVsActivityLog m_Log;
        private readonly IVsStatusbar m_StatusBar;

        public WinHelper(IVsActivityLog log, IVsStatusbar statusBar)
        {
            Ensure.That(() => log).IsNotNull();
            Ensure.That(() => statusBar).IsNotNull();
            m_Log = log;
            m_StatusBar = statusBar;
        }


        /// <summary>
        /// Starts service if it's stopped. Returns True if service was started.
        /// </summary>
        public bool StartServiceIfNeeded(string serviceName, int timeout = 30)
        {
            Ensure.That(() => serviceName).IsNotNullOrEmpty();
            if (ServiceIsRunning(serviceName))
                return false;

            using (ServiceController sc = new ServiceController(serviceName))
            {
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    NotifyInStatusBar($"Starting {serviceName}");
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                }
                if (sc.Status == ServiceControllerStatus.StartPending)
                {
                    NotifyInStatusBar($"Starting {serviceName}");
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(timeout));
                }
            }

            NotifyInStatusBar($"Service {serviceName} has started.");
            return true;
        }

        /// <summary>
        /// Stops service if it's running. Returns True if service was stopped.
        /// </summary>
        public bool StopServiceIfNeeded(string serviceName, int timeout = 30)
        {
            Ensure.That(() => serviceName).IsNotNullOrEmpty();
            if (!ServiceIsRunning(serviceName))
                return false;

            using (ServiceController sc = new ServiceController(serviceName))
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    NotifyInStatusBar($"Stopping {serviceName} ...");
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                if (sc.Status == ServiceControllerStatus.StopPending)
                {
                    NotifyInStatusBar($"Stopping {serviceName} ...");
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(timeout));
                }
            }

            NotifyInStatusBar($"Service {serviceName} has stoped.");
            return true;
        }

        public bool StartProcessIfNeeded(string name, int timeout = 30)
        {
            Ensure.That(() => name).IsNotNullOrEmpty();
            var runningProcesses = Process.GetProcessesByName(name);
            if (runningProcesses.Length == 0)
            {
                try
                {
                    NotifyInStatusBar($"Starting process {name} ...");
                    var process = Process.Start(name);
                    return true;
                }
                catch (Exception ex)
                {
                    NotifyInStatusBar($"Failed to start process {name} more information exists in the activity log.");
                    WriteToLog(ex);
                }
            }

            return false;
        }

        public bool StopProcessIfNeeded(string name, int timeout = 30)
        {
            Ensure.That(() => name).IsNotNullOrEmpty();
            var runningProcesses = Process.GetProcessesByName(name);
            if (runningProcesses.Length == 0)
                return false;

            foreach (var process in runningProcesses)
            {
                try
                {
                    NotifyInStatusBar($"Killing process {name} ...");
                    process.Kill();
                }
                catch (Exception ex)
                {
                    NotifyInStatusBar($"Failed to kill process {name} more information exists in the activity log.");
                    WriteToLog(ex);
                }
            }

            return true;
        }

        private bool ServiceIsRunning(string serviceName)
        {
            ServiceController[] scServices = null;
            try
            {
                scServices = ServiceController.GetServices();

                var service = scServices.SingleOrDefault(s =>
                    s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

                if (service == null)
                    return false;

                return service.Status != ServiceControllerStatus.Stopped;
            }
            finally
            {
                if (scServices != null)
                    foreach (var service in scServices)
                        service.Dispose();
            }
        }

        private void UsingService(string serviceName, Action<ServiceController> controllerAction)
        {
            ServiceController sc = null;
            try
            {
                sc = new ServiceController(serviceName);
                controllerAction(sc);
            }
            catch (Exception ex)
            {
                WriteToLog(ex);
            }
            finally
            {
                if (sc != null)
                    sc.Dispose();
            }
        }

        private void NotifyInStatusBar(string message)
        {
            // Make sure the status bar is not frozen
            int frozen;
            m_StatusBar.IsFrozen(out frozen);
            if (frozen != 0)
            {
                m_StatusBar.FreezeOutput(0);
            }

            // Set the status bar text and make its display static.
            m_StatusBar.SetText(message);

            // Freeze the status bar.
            m_StatusBar.FreezeOutput(1);

            // Clear the status bar text.
            m_StatusBar.FreezeOutput(0);
            m_StatusBar.Clear();
        }

        private void WriteToLog(Exception ex)
        {
            int hr = m_Log.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                this.ToString(), "Error has occured: " + ex.Message + "\n Stack trace: " + ex.StackTrace);
        }
    }
}
