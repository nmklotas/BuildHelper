using BuildHelper.UI;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using Newtonsoft.Json;
using System;

namespace BuildHelper.Settings
{
    public class BuildHelperSettings
    {
        private const string PropertyName = "Settings";
        private static readonly string s_CollectionPath;
        private readonly Lazy<WritableSettingsStore> m_Store;

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
                return null;

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
