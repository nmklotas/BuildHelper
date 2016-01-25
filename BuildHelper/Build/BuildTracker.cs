using BuildHelper.CodeGuard;
using BuildHelper.Settings;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;

namespace BuildHelper.Build
{
    partial class BuildTracker : IVsUpdateSolutionEvents2
    {
        private readonly IVsSolutionBuildManager2 m_BuildManager;
        private readonly BuildHelperSettings m_Settings;
        private readonly DTE2 m_VsInstance;
        private bool m_ProcessStoped = false;
        private bool m_ServiceStoped = false;
        private readonly WinHelper m_WinHelper;

        public BuildTracker(DTE2 vsInstance, IVsSolutionBuildManager2 buildManager, BuildHelperSettings settings, WinHelper winHelper)
        {
            Guard.That(vsInstance).IsNotNull();
            Guard.That(settings).IsNotNull();
            Guard.That(winHelper).IsNotNull();
            Guard.That(buildManager).IsNotNull();

            m_VsInstance = vsInstance;
            m_Settings = settings;
            m_WinHelper = winHelper;

            uint pdwCookieSolutionBM;
            m_BuildManager = buildManager;
            m_BuildManager.AdviseUpdateSolutionEvents(this, out pdwCookieSolutionBM);
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            var optionsDataSource = m_Settings.Load();
            optionsDataSource.UsingOption(GetSolution(), opt =>
            {
                if (!string.IsNullOrEmpty(opt.ProcessName))
                    m_ProcessStoped |= m_WinHelper.StopProcessIfNeeded(opt.GetSimpleProcessName());

                if (!string.IsNullOrEmpty(opt.ServiceName))
                    m_ServiceStoped |= m_WinHelper.StopServiceIfNeeded(opt.ServiceName);
            });

            return VSConstants.S_OK;
        }

        public int UpdateSolution_Cancel()
        {
            OnBuildEnd();
            return VSConstants.S_OK;
        }

        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            OnBuildEnd();
            return VSConstants.S_OK;
        }

        private void OnBuildEnd()
        {
            var optionsDataSource = m_Settings.Load();
            optionsDataSource.UsingOption(GetSolution(), opt =>
            {
                if (!string.IsNullOrEmpty(opt.ProcessName))
                    if (m_ProcessStoped)
                        m_WinHelper.StartProcessIfNeeded(opt.ProcessName);

                if (!string.IsNullOrEmpty(opt.ServiceName))
                    if (m_ServiceStoped)
                        m_WinHelper.StartServiceIfNeeded(opt.ServiceName);
            });

            //revert values for each solution build
            m_ProcessStoped = false;
            m_ServiceStoped = false;
        }

        private string GetSolution()
        {
            string fileName = Path.GetFileName(m_VsInstance.Solution.FileName);
            return fileName;
        }
    }
}
