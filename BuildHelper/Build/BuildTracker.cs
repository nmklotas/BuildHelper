using System.Collections.Generic;
using System.IO;
using BuildHelper.OS;
using BuildHelper.Settings;
using EnsureThat;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildHelper.Build
{
	internal partial class BuildTracker : IVsUpdateSolutionEvents2
	{
		private readonly IVsSolutionBuildManager2 m_BuildManager;
		private readonly BuildHelperSettings m_Settings;
		private readonly DTE2 m_VsInstance;
		private readonly HashSet<TrackInfo> m_StopedProcesses = new HashSet<TrackInfo>(TrackInfoEqualityComparer.Default);
		private readonly HashSet<TrackInfo> m_StopedServices = new HashSet<TrackInfo>(TrackInfoEqualityComparer.Default);
		private readonly WinHelper m_WinHelper;

		public BuildTracker(
			DTE2 vsInstance,
			IVsSolutionBuildManager2 buildManager,
			BuildHelperSettings settings,
			WinHelper winHelper)
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

			optionsDataSource.WithConfiguration(GetSolution(), config =>//get configuration for the given solution
			{
				foreach (string process in config.ParseProcessPaths())
				{
					if (m_WinHelper.StopProcessIfNeeded(process))
					{
						var trackInfo = new TrackInfo(process, config.RestartProcess);
						if (!m_StopedProcesses.Contains(trackInfo))
							m_StopedProcesses.Add(trackInfo);
					}
				}
				foreach (string service in config.ParseWindowsServicesNames())
				{
					if (m_WinHelper.StopServiceIfNeeded(service))
					{
						var trackInfo = new TrackInfo(service, config.RestartService);
						if (!m_StopedServices.Contains(trackInfo))
							m_StopedServices.Add(trackInfo);
					}
				}
			});

			return VSConstants.S_OK;
		}

		public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
		{
			if (fSucceeded == 1)
			{
				try
				{
					foreach (TrackInfo process in m_StopedProcesses)
					{
						if (process.Restart)
							m_WinHelper.StartProcessIfNeeded(process.Id);
					}

					foreach (TrackInfo service in m_StopedServices)
					{
						if (service.Restart)
							m_WinHelper.StartServiceIfNeeded(service.Id);
					}
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
