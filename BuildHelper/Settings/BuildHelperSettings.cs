using BuildHelper.UI;
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

        private static OptionsDataSource s_DefaultDataSource = new OptionsDataSource()
        {
            Options = new ObservableCollection<Option>
            {
                #region Default settings

                new Option
                {
                    SolutionName = "Desktop.sln",
                    ProcessName = @"C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcMap.exe",
                    ServiceName = "ArcGIS Server"
                },
                new Option
                {
                    SolutionName = "Toolboxes.sln",
                    ProcessName = @"C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcMap.exe",
                    ServiceName = "ArcGIS Server"
                },
                new Option
                {
                    SolutionName = "Extensions.sln",
                    ProcessName = @"C:\Program Files (x86)\ArcGIS\Desktop10.2\bin\ArcMap.exe",
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

        public void Save(OptionsDataSource options)
        {
            if (!m_Store.Value.CollectionExists(s_CollectionPath))
                m_Store.Value.CreateCollection(s_CollectionPath);

            m_Store.Value.SetString(s_CollectionPath, PropertyName, Serialize(options));
        }

        public OptionsDataSource Load()
        {
            if (!m_Store.Value.CollectionExists(s_CollectionPath))
                return s_DefaultDataSource;

            return Deserialize(m_Store.Value.GetString(s_CollectionPath, PropertyName));
        }

        private string Serialize(OptionsDataSource options)
        {
            return JsonConvert.SerializeObject(options);
        }

        private OptionsDataSource Deserialize(string options)
        {
            return JsonConvert.DeserializeObject<OptionsDataSource>(options);
        }
    }
}
