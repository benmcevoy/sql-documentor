using System.Windows;
using SQLDocumentor.ViewModels;

namespace SQLDocumentor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window
    {
        private GeneratorViewModel _viewModel;
        public GeneratorViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                this.DataContext = _viewModel;
            }
        }

        public Shell()
        {
            InitializeComponent();
        }
    }
}
