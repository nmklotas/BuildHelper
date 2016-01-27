using BuildHelper.UI.Data;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace BuildHelper.UI
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("1D9ECCF3-5D2F-4112-9B25-264596873DC9")]
    public class BuildHelperOptions : UIElementDialogPage, IBuildConfigurationDataSourceProvider
    {
        BuildConfigurationDataSource m_OptionsDatasource;

        public BuildHelperOptions()
        {
            m_OptionsDatasource = BuildHelperPackage.Settings.Load();
        }

        public BuildConfigurationDataSource DataSource
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
            BuildHelperPackage.Settings.Save(m_OptionsDatasource);
            base.OnClosed(e);
        }
    }
}
