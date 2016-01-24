using System.Windows.Controls;

namespace BuildHelper.UI
{
    /// <summary>
    /// Interaction logic for BuildHelperOptionsUserControl.xaml
    /// </summary>
    public partial class BuildHelperOptionsUserControl : UserControl
    {
        public BuildHelperOptionsUserControl(BuildHelperOptions options)
        {
            InitializeComponent();
            this.DataContext = options;
        }
    }
}
