using System.Collections.ObjectModel;

namespace BuildHelper.UI.Data
{
    public class DesignTimeOptionsDataSource
    {
        public ObservableCollection<Option> Options = new ObservableCollection<Option>
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
