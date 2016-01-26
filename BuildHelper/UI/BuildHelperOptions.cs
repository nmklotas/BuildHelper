using EnsureThat;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace BuildHelper.UI
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("1D9ECCF3-5D2F-4112-9B25-264596873DC9")]
    public class BuildHelperOptions : UIElementDialogPage
    {
        OptionsDataSource m_OptionsDatasource;

        public BuildHelperOptions()
        {
            m_OptionsDatasource = BuildHelperPackage.Settings.Load();
        }

        public OptionsDataSource Options
        {
            get { return m_OptionsDatasource; }
            set { m_OptionsDatasource = value; }
        }

        protected override UIElement Child
        {
            get
            {
                return new BuildHelperOptionsUserControl(this);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            BuildHelperPackage.Settings.Save(m_OptionsDatasource);
        }
    }

    [Serializable]
    public class OptionsDataSource
    {
        public ObservableCollection<Option> Options { get; set; } = new ObservableCollection<Option>();

        public void UsingOption(string solutionName, Action<Option> optionsAction)
        {
            Ensure.That(() => solutionName).IsNotNullOrEmpty();
            Ensure.That(() => optionsAction).IsNotNull();

            foreach (var opt in Options)
            {
                if (opt.SolutionName.Equals(solutionName, StringComparison.OrdinalIgnoreCase))
                    optionsAction(opt);
            }
        }
    }

    [Serializable]
    public class Option
    {
        public string SolutionName { get; set; }

        public string ProcessName { get; set; }

        public string ServiceName { get; set; }

        public string GetSimpleProcessName()
        {
            return Path.GetFileNameWithoutExtension(ProcessName);
        }
    }

    public class DesignTimeOptionsDataSource
    {
        public List<Option> Options = new List<Option>
        {
            new Option
            {
                SolutionName = "Test.sln",
                ProcessName = "Notepad.exe",
                ServiceName = "Service"
            }
        };
    }
}
