using System;
using SQLDocumentor.Infrastructure;
using System.Windows.Input;
using System.Collections.ObjectModel;
using SQLDocumentor.Services;
using System.ComponentModel;

namespace SQLDocumentor.ViewModels
{
    public class GeneratorViewModel : INotifyPropertyChanged
    {
        private readonly IGeneratorService _generator;

        public event EventHandler GenerateComplete;

        public GeneratorViewModel(IGeneratorService generator)
        {
            _generator = generator;
           
            GenerateCommand = new RelayCommand(param => Generate(), param => CanGenerate);
            GetDatabasesCommand = new RelayCommand(param => GetDatabases(), param => CanGetDatabases);

            RendererName = _generator.Renderer.Name;
            RendererDescription = _generator.Renderer.Description;

            ServerName = _generator.Server.Name;
            ServerDescription = _generator.Server.Description;
        }

        private void GetDatabases()
        {
            _generator.Server.ServerName = Server;
            _generator.Server.DatabaseName = Database;
            _generator.Server.UserName = UserName;
            _generator.Server.UseIntegratedSecurity = UseIntegratedSecurity;
            _generator.Server.Password = Password;

            Databases = new ObservableCollection<string>(_generator.Server.GetDatabases());
            OnPropertyChanged("Databases");
        }

        private void Generate()
        {
            _generator.Server.ServerName = Server;
            _generator.Server.DatabaseName = Database;
            _generator.Server.UserName = UserName;
            _generator.Server.UseIntegratedSecurity = UseIntegratedSecurity;
            _generator.Server.Password = Password;

            Status = "Generate Complete";

            _generator.Generate();

            Status = "Generate Complete";

            OnGenerateComplete();
        }

        private void OnGenerateComplete()
        {
            if (GenerateComplete != null)
            {
                GenerateComplete(this, new EventArgs());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool CanGetDatabases
        {
            // TODO: should validate server, etc
            get { return true; }
        }

        private bool CanGenerate
        {
            // TODO: should validate server, etc
            get { return true; }
        }

        #region Binding

        private string _status = "";
        public string Status { get { return _status; } set { _status = value; OnPropertyChanged("Status"); } }

        public ICommand GenerateCommand { get; private set; }
        public ICommand GetDatabasesCommand { get; private set; }

        private string _server = "(local)";
        public string Server { get { return _server; } set { _server = value; OnPropertyChanged("Server"); } }

        private string _database = "";
        public string Database { get { return _database; } set { _database = value; OnPropertyChanged("Database"); } }

        private string _userName = "";
        public string UserName { get { return _userName; } set { _userName = value; OnPropertyChanged("UserName"); } }

        private string _password = "";
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged("Password"); } }

        private bool _useIntegratedSecurity = true;
        public bool UseIntegratedSecurity { get { return _useIntegratedSecurity; } set { _useIntegratedSecurity = value; OnPropertyChanged("UseIntegratedSecurity"); } }

        public ObservableCollection<string> Databases { get; set; }

        private string _serverName;
        public string ServerName { get { return _serverName; } set { _serverName = value; OnPropertyChanged("ServerName"); } }

        private string _serverDescription;
        public string ServerDescription { get { return _serverDescription; } set { _serverDescription = value; OnPropertyChanged("ServerDescription"); } }

        private string _rendererName;
        public string RendererName { get { return _rendererName; } set { _rendererName = value; OnPropertyChanged("RendererName"); } }

        private string _rendererDescription;
        public string RendererDescription { get { return _rendererDescription; } set { _rendererDescription = value; OnPropertyChanged("RendererDescription"); } }

        #endregion

    }
}

