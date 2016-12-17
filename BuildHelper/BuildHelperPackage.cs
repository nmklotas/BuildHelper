//------------------------------------------------------------------------------
// <copyright file="BuildHelperPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using BuildHelper.Build;
using BuildHelper.Log;
using BuildHelper.OS;
using BuildHelper.Settings;
using BuildHelper.UI;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;

namespace BuildHelper
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[Guid(BuildHelperPackage.PackageGuidString)]
	[ProvideOptionPage(typeof(BuildHelperOptions), "BuildHelper", "BuildHelper options", 0, 0, true)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
	public sealed class BuildHelperPackage : Package
	{
		/// <summary>
		/// GFAHelperPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "85888a11-fe6f-413d-991f-5e5cfbd3b394";
		private BuildTracker m_BuildTracker;
		private DTE2 m_VsInstance;

		/// <summary>
		/// Settings for all package
		/// </summary>
		public static BuildHelperSettings Settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="BuildHelperPackage"/> class.
		/// </summary>
		public BuildHelperPackage()
		{
			// Inside this method you can place any initialization code that does not require
			// any Visual Studio service because at this point the package object is created but
			// not sited yet inside Visual Studio environment. The place to do all the other
			// initialization is the Initialize method.
		}

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			var settingsStore = new Lazy<WritableSettingsStore>(() =>
			{
				return new ShellSettingsManager(this).
					GetWritableSettingsStore(SettingsScope.UserSettings);
			});

			Settings = new BuildHelperSettings(settingsStore);
			m_VsInstance = (DTE2)GetService(typeof(DTE));
			var solutionBuildManager = (IVsSolutionBuildManager2)GetService(typeof(SVsSolutionBuildManager));
			var statusBar = (IVsStatusbar)GetService(typeof(SVsStatusbar));
			var logger = new ExtensionLogger(() => GetService(typeof(SVsActivityLog)) as IVsActivityLog, statusBar);
            //never cache the activity log reference
            var serviceHelper = new ServiceHelper(logger); 
            var processHelper = new ProcessHelper(logger);
			m_BuildTracker = new BuildTracker(
                m_VsInstance, 
                solutionBuildManager, 
                Settings, 
                serviceHelper, 
                processHelper);
		}
	}
}

