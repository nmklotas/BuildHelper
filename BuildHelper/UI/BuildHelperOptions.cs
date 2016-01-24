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
        private static OptionsDataSource DefaultDataSource = new OptionsDataSource()
        {
            Options = new ObservableCollection<Option>()
            {
                new Option()
                {
                    SolutionName = "Test.sln",
                    ProcessName = "Notepad.exe",
                    ServiceName = "Service"
                }
            }
        };

        public BuildHelperOptions()
        {
            m_OptionsDatasource = BuildHelperPackage.Settings.Load() ?? DefaultDataSource;
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

        public bool HasSolution(string name, out Option option)
        {
            option = null;

            foreach (var opt in Options)
            {
                if (opt.SolutionName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    option = opt;
                    return true;
                }
            }

            return false;
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
        public List<Option> Options = new List<Option>()
        {
            new Option()
            {
                SolutionName = "Test.sln",
                ProcessName = "Notepad.exe",
                ServiceName = "Service"
            }
        };
    }
}
