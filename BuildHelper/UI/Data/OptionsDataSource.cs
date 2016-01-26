using EnsureThat;
using System;
using System.Collections.ObjectModel;

namespace BuildHelper.UI.Data
{
    [Serializable]
    public class OptionsDataSource
    {
        public ObservableCollection<Option> Options { get; set; } = new ObservableCollection<Option>();

        public void UsingOption(string solutionName, Action<Option> optionsAction)
        {
            Ensure.That(() => solutionName).IsNotNullOrEmpty();
            Ensure.That(() => optionsAction).IsNotNull();

            foreach (var opt in Options)
            {
                if (opt.SolutionName.Equals(solutionName, StringComparison.OrdinalIgnoreCase))
                    optionsAction(opt);
            }
        }
    }
}
