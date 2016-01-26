using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildHelper.UI.Data
{
    [Serializable]
    public class Option
    {
        public string SolutionName { get; set; }

        public string ProcessName { get; set; }

        public string ServiceName { get; set; }

        public List<string> ParseProcessPaths()
        {
            return Parse(ProcessName).ToList();
        }

        public List<string> ParseWindowsServicesNames()
        {
            return Parse(ServiceName).ToList();
        }

        private string[] Parse(string property)
        {
            Ensure.That(() => property).IsNotNullOrWhiteSpace();
            return property.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
