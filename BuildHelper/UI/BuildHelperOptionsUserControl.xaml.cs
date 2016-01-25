using System.Windows.Controls;

namespace BuildHelper.UI
{
    /// <summary>
    /// Interaction logic for BuildHelperOptionsUserControl.xaml
    /// </summary>
    public partial class BuildHelperOptionsUserControl : UserControl
    {
        private BuildHelperOptions m_Options;

        public BuildHelperOptionsUserControl(BuildHelperOptions options)
        {
            InitializeComponent();
            m_Options = options;
            this.DataContext = m_Options;
        }

        private void AddButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            m_Options.Options.Options.Add(new Option());
        }

        private void RemoveButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedOption = m_DgOptions.SelectedItem as Option;
            if (selectedOption != null)
            {
                m_Options.Options.Options.Remove(selectedOption);
            }
        }
        /*
        private void RefreshDataSource()
        {
            var bindingExpression = m_DgOptions.GetBindingExpression(DataGrid.ItemsSourceProperty);
            if (bindingExpression != null)
                bindingExpression.UpdateSource();
        }*/
    }
}
