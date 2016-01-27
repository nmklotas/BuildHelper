using EnsureThat;
using System;
using System.Collections.ObjectModel;

namespace BuildHelper.UI.Data
{
    /// <summary>
    /// Class used to store configuration settings in Visual Studio ConfigurationStore
    /// </summary>
    [Serializable]
    public class BuildConfigurationDataSource
    {
        public ObservableCollection<BuildConfiguration> Configuration { get; set; } = new ObservableCollection<BuildConfiguration>();

        public void WithConfiguration(
            string solutionName,
            Action<BuildConfiguration> optionsAction)
        {
            Ensure.That(() => solutionName).IsNotNullOrEmpty();
            Ensure.That(() => optionsAction).IsNotNull();

            foreach (var config in Configuration)
            {
                if (config.SolutionName.Equals(solutionName, StringComparison.OrdinalIgnoreCase))
                    optionsAction(config);
            }
        }
    }
}
