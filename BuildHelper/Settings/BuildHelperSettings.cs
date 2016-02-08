using BuildHelper.UI.Data;
using EnsureThat;
using Microsoft.VisualStudio.Settings;
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
                    ServiceName = "ArcGIS Server",
                    RestartService = true,
                    RestartProcess = true
                },
                new BuildConfiguration
                {
                    SolutionName = "Toolboxes.sln",
                    ProcessName = @"C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcMap.exe;C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcCatalog.exe",
                    ServiceName = "ArcGIS Server",
                    RestartService = true,
                    RestartProcess = true
                },
                new BuildConfiguration
                {
                    SolutionName = "Extensions.sln",
                    ProcessName = @"C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcMap.exe;C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcCatalog.exe",
                    ServiceName = "ArcGIS Server",
                    RestartService = true,
                    RestartProcess = true
                },
                new BuildConfiguration
                {
                    SolutionName = "Patch.sln",
                    ProcessName = "DbTool",
                    RestartService = false,
                    RestartProcess = false
                }

                #endregion
            }
        };

        static BuildHelperSettings()
        {
            s_CollectionPath = typeof(BuildHelperSettings).FullName;
        }

        public BuildHelperSettings(Lazy<WritableSettingsStore> settingsStore)
        {
            Ensure.That(() => settingsStore).IsNotNull();
            m_Store = settingsStore;
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
