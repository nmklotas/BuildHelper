using BuildHelper.Settings;
using BuildHelper.UI;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
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

        public BuildTracker(DTE2 vsInstance, BuildHelperSettings settings)
        {
            m_VsInstance = vsInstance;
            m_Settings = settings;

            uint pdwCookieSolutionBM;
            m_BuildManager = (IVsSolutionBuildManager2)ServiceProvider.GlobalProvider.GetService(
                typeof(SVsSolutionBuildManager));
            m_BuildManager.AdviseUpdateSolutionEvents(this, out pdwCookieSolutionBM);
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            var options = m_Settings.Load();

            Option option;
            if (options.HasSolution(GetSolution(), out option))
            {
                if (!string.IsNullOrEmpty(option.ProcessName))
                    m_ProcessStoped = WinHelper.StopProcessIfNeeded(option.ProcessName);

                if (!string.IsNullOrEmpty(option.ServiceName))
                    m_ServiceStoped = WinHelper.StopServiceIfNeeded(option.ServiceName);
            }

            return 0;
        }

        public int UpdateSolution_Cancel()
        {
            OnBuildEnd();
            return 0;
        }

        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            OnBuildEnd();
            return 0;
        }

        private void OnBuildEnd()
        {
            var options = m_Settings.Load();

            Option option;
            if (options.HasSolution(GetSolution(), out option))
            {
                if (!string.IsNullOrEmpty(option.ProcessName))
                    if (m_ProcessStoped)
                        WinHelper.StartProcessIfNeeded(option.ProcessName);

                if (!string.IsNullOrEmpty(option.ServiceName))
                    if (m_ServiceStoped)
                        WinHelper.StartServiceIfNeeded(option.ServiceName);
            }

            //revert values for each build
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
