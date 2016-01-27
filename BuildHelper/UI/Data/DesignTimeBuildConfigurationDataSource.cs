using System.Collections.ObjectModel;

namespace BuildHelper.UI.Data
{
    public class DesignTimeBuildConfigurationDataSource
    {
        public ObservableCollection<BuildConfiguration> Configuration = new ObservableCollection<BuildConfiguration>
        {
            new BuildConfiguration
            {
                SolutionName = "Test.sln",
                ProcessName = "Notepad.exe",
                ServiceName = "Service"
            }
        };
    }
}
