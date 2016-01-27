using BuildHelper.UI.Data;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace BuildHelper.Settings
{
    public class BuildHelperSettings
    {
        private const string PropertyName = "Settings";
        private static readonly string s_CollectionPath;
        private readonly Lazy<WritableSettingsStore> m_Store;

        private static BuildConfigurationDataSource s_DefaultDataSource = new BuildConfigurationDataSource()
        {
            Configuration = new ObservableCollection<BuildConfiguration>
            {
                #region Default settings

                new BuildConfiguration
                {
                    SolutionName = "Desktop.sln",
                    ProcessName = @"C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcMap.exe;C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcCatalog.exe",
                    ServiceName = "ArcGIS Server"
                },
                new BuildConfiguration
                {
                    SolutionName = "Toolboxes.sln",
                    ProcessName = @"C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcMap.exe;C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcCatalog.exe",
                    ServiceName = "ArcGIS Server"
                },
                new BuildConfiguration
                {
                    SolutionName = "Extensions.sln",
                    ProcessName = @"C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcMap.exe;C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcCatalog.exe",
                    ServiceName = "ArcGIS Server"
                }

                 #endregion
            }
        };

        static BuildHelperSettings()
        {
            s_CollectionPath = typeof(BuildHelperSettings).FullName;
        }

        public BuildHelperSettings(IServiceProvider serviceProvider)
        {
            m_Store = new Lazy<WritableSettingsStore>(() =>
            {
                return new ShellSettingsManager(serviceProvider).
                    GetWritableSettingsStore(SettingsScope.UserSettings);
            });
        }

        public void Save(BuildConfigurationDataSource options)
        {
            if (!m_Store.Value.CollectionExists(s_CollectionPath))
                m_Store.Value.CreateCollection(s_CollectionPath);

            m_Store.Value.SetString(s_CollectionPath, PropertyName, Serialize(options));
        }

        public BuildConfigurationDataSource Load()
        {
            if (!m_Store.Value.CollectionExists(s_CollectionPath))
                return s_DefaultDataSource;

            return Deserialize(m_Store.Value.GetString(s_CollectionPath, PropertyName));
        }

        private string Serialize(BuildConfigurationDataSource options)
        {
            return JsonConvert.SerializeObject(options);
        }

        private BuildConfigurationDataSource Deserialize(string options)
        {
            return JsonConvert.DeserializeObject<BuildConfigurationDataSource>(options);
        }
    }
}
