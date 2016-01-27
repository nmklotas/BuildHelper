using BuildHelper.UI.Data;

namespace BuildHelper.UI
{
    public interface IBuildConfigurationDataSourceProvider
    {
        BuildConfigurationDataSource DataSource { get; set; }
    }
}