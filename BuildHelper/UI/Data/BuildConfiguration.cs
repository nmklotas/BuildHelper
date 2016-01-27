using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildHelper.UI.Data
{
    [Serializable]
    public class BuildConfiguration
    {
        public string SolutionName { get; set; }

        public string ProcessName { get; set; }

        public string ServiceName { get; set; }

        public List<string> ParseProcessPaths()
        {
            if (string.IsNullOrEmpty(ProcessName))
                return new List<string>();

            return Parse(ProcessName);
        }

        public List<string> ParseWindowsServicesNames()
        {
            if (string.IsNullOrEmpty(ServiceName))
                return new List<string>();

            return Parse(ServiceName);
        }

        private List<string> Parse(string property)
        {
            return property.Split(
                new string[] { ";" },
                StringSplitOptions.RemoveEmptyEntries).
                Select(s => s.Trim()).ToList();
        }
    }
}
