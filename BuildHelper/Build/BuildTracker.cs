using BuildHelper.Settings;
using EnsureThat;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;

namespace BuildHelper.Build
{
	partial class BuildTracker : IVsUpdateSolutionEvents2
	{
		private readonly IVsSolutionBuildManager2 m_BuildManager;
		private readonly BuildHelperSettings m_Settings;
		private readonly DTE2 m_VsInstance;
		private readonly HashSet<string> m_StopedProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly HashSet<string> m_StopedServices = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly WinHelper m_WinHelper;

		public BuildTracker(DTE2 vsInstance, IVsSolutionBuildManager2 buildManager, BuildHelperSettings settings, WinHelper winHelper)
		{
			Ensure.That(() => vsInstance).IsNotNull();
			Ensure.That(() => settings).IsNotNull();
			Ensure.That(() => winHelper).IsNotNull();
			Ensure.That(() => buildManager).IsNotNull();

			m_VsInstance = vsInstance;
			m_Settings = settings;
			m_WinHelper = winHelper;

			uint pdwCookieSolutionBM;
			m_BuildManager = buildManager;
			m_BuildManager.AdviseUpdateSolutionEvents(this, out pdwCookieSolutionBM);
		}

		public int UpdateSolution_Begin(ref int pfCancelUpdate)
		{
			//todo: maybe add option to by using ref int pfCancelUpdate?
			var optionsDataSource = m_Settings.Load();

			optionsDataSource.UsingOption(GetSolution(), opt =>
			{
				//get option for configured solution
				foreach (string process in opt.ParseProcessPaths())
				{
					if (m_WinHelper.StopProcessIfNeeded(process))
						if (!m_StopedProcesses.Contains(process))
							m_StopedProcesses.Add(process);
				}
				foreach (string service in opt.ParseWindowsServicesNames())
				{
					if (m_WinHelper.StopServiceIfNeeded(service))
						if (!m_StopedServices.Contains(service))
							m_StopedServices.Add(service);
				}
			});

			return VSConstants.S_OK;
		}

		public int UpdateSolution_Cancel()
		{
			//if the build is canceled do nothing until it succeeds
			return VSConstants.S_OK;
		}

		public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
		{
			if (fSucceeded == 1)
			{
				try
				{
					foreach (string process in m_StopedProcesses)
						m_WinHelper.StartProcessIfNeeded(process);

					foreach (string service in m_StopedServices)
						m_WinHelper.StartServiceIfNeeded(service);
				}
				finally
				{
					//clean stored stoped services/processes for each solution build
					//if it succeeds or exception occurs when restarting services/processes
					m_StopedProcesses.Clear();
					m_StopedServices.Clear();
				}
			}

			return VSConstants.S_OK;
		}

		private string GetSolution()
		{
			string fileName = Path.GetFileName(m_VsInstance.Solution.FileName);
			return fileName;
		}
	}
}
