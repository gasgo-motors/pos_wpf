using BusinessLayer;
using DataLayer;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Linq;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Configuration = System.Configuration.Configuration;

namespace PosClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static SqlConnection LocalConnection;
        public static new App Current
        {
            get
            {
                return Application.Current as App;
            }
        }

        public MainWindow CurrentMainWindow
        {
            get
            {
                return Application.Current.MainWindow as MainWindow;
            }
        }

        public async Task<bool> ShowConfirmMessage(string title, string message)
        {
            return
                await
                    CurrentMainWindow.ShowMessageAsync(title, message,
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative;
        }

        public PosUser User
        {
            get
            {
                return PosUsersManager.Current.User;
            }
        }

        public Setting PosSetting
        {
            get
            {
                return SettingsManager.Current.CurrentSetting;
            }
        }

        public byte[] MotivationPicture
        {
            get
            {
                return SettingsManager.Current.MotivationPicture;
            }
        }

        public async void ShowErrorDialog(string title, string errorMessage)
        {
            CurrentMainWindow.ShowCutomErrorDialog(title, errorMessage);
        }

        public FrameworkElement TargetTextBox { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);
            if (!EnsureSingleInstance())
            {
                this.Shutdown();
            }
            else
            {
                try
                {
                    string filePath = "C:\\Wr\\settings.xml";
                    if (System.IO.File.Exists(filePath))
                    {
                        XDocument xDoc = XDocument.Load(filePath);
                        String datasource = xDoc.Element("root").Element("datasource").Value;
                        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        EntityConnectionStringBuilder efb = new EntityConnectionStringBuilder(config.ConnectionStrings.ConnectionStrings["POSWR1Entities"].ConnectionString);
                        SqlConnectionStringBuilder sqb = new SqlConnectionStringBuilder(efb.ProviderConnectionString);
                        sqb.DataSource = datasource;
                        efb.ProviderConnectionString = sqb.ConnectionString;
                        config.ConnectionStrings.ConnectionStrings["POSWR1Entities"].ConnectionString = efb.ConnectionString;

                        config.Save(ConfigurationSaveMode.Modified, true);
                        ConfigurationManager.RefreshSection("connectionStrings");

                        App.LocalConnection = new SqlConnection(sqb.ToString());

                        executeScript();
                    }
                    new MainWindow().ShowDialog();
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "სისტემური შეცდომა" );
                   // App.Current.ShowErrorDialog("შეცდომა", ex.Message);
                }
                this.Shutdown();
            }
        }


        public static bool executeScript()
        {
            try
            {
                StreamResourceInfo i = Application.GetResourceStream(new Uri("/Resources/VersionScript.sql", UriKind.RelativeOrAbsolute));
                StreamReader reader = new StreamReader(i.Stream);
                string text = reader.ReadToEnd();
                ExecuteDBScript(App.LocalConnection, text);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool ExecuteDBScript(SqlConnection _sc, string commandText)
        {
            try
            {
                Server server = new Server(new ServerConnection(_sc));
                server.ConnectionContext.ExecuteNonQuery(commandText);
            }
            catch (Exception ex)
            {
                _sc.Close();
                return false;
            }
            finally { _sc.Close(); }
            return true;
        }


        static bool EnsureSingleInstance()
        {

            Process currentProcess = Process.GetCurrentProcess();

            var runningProcess = (from process in Process.GetProcesses()
                                  where
                                    process.Id != currentProcess.Id &&
                                    process.ProcessName.Equals(
                                      currentProcess.ProcessName,
                                      StringComparison.Ordinal)
                                  select process).FirstOrDefault();
            if (runningProcess != null)
            {
                return false;
            }
            return true;
        }



    }
}
