using Microsoft.VisualStudio.Shell.Interop;

namespace BuildHelper.Build
{
    partial class BuildTracker
    {
        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return 0;
        }

        public int UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            return 0;
        }

        public int UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            return 0;
        }

        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return 0;
        }
    }
}
