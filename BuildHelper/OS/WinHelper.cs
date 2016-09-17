﻿using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using BuildHelper.Log;
using EnsureThat;
using Process = System.Diagnostics.Process;

namespace BuildHelper.OS
{
	internal class WinHelper
	{
		private readonly IExtensionLogger m_ExtensionLogger;

		public WinHelper(IExtensionLogger extensionLogger)
		{
			Ensure.That(() => extensionLogger).IsNotNull();
			m_ExtensionLogger = extensionLogger;
		}

		/// <summary>
		/// Starts service if it's stopped. Returns True if service was started.
		/// </summary>
		public bool StartServiceIfNeeded(string serviceName, int timeout = 30)
		{
			Ensure.That(() => serviceName).IsNotNullOrEmpty();
			if (ServiceIsRunning(serviceName))
				return false;

			ServiceController sc = null;
			try
			{
				sc = new ServiceController(serviceName);
				if (sc.Status == ServiceControllerStatus.Stopped)
				{
					WriteStatus($"Starting {serviceName}");
					sc.Start();
					sc.WaitForStatus(ServiceControllerStatus.Running);
				}
				if (sc.Status == ServiceControllerStatus.StartPending)
				{
					WriteStatus($"Starting {serviceName}");
					sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(timeout));
				}
			}
			catch (Exception ex)
			{
				WriteException(ex);
				WriteStatus($"Failed to start service {serviceName} more information exists in the activity log.");
				return false;
			}
			finally
			{
				if (sc != null)
					sc.Dispose();
			}


			WriteStatus($"Service {serviceName} has started.");
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

			ServiceController sc = null;
			try
			{
				sc = new ServiceController(serviceName);
				if (sc.Status == ServiceControllerStatus.Running)
				{
					WriteStatus($"Stopping {serviceName} ...");
					sc.Stop();
					sc.WaitForStatus(ServiceControllerStatus.Stopped);
				}
				if (sc.Status == ServiceControllerStatus.StopPending)
				{
					WriteStatus($"Stopping {serviceName} ...");
					sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(timeout));
				}
			}
			catch (Exception ex)
			{
				WriteException(ex);
				WriteStatus($"Failed to stop service {serviceName} more information exists in the activity log.");
				return false;
			}
			finally
			{
				if (sc != null)
					sc.Dispose();
			}

			WriteStatus($"Service {serviceName} has stoped.");
			return true;
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
