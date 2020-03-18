using BusinessLayer;
using DataLayer;
using Microsoft.Win32;
using PosClient.Helpers;
using PosClient.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace PosClient.Views
{
    public abstract class SettingsController : PosUserControl<Settings, SettingsViewModel>
    {
         
    }


    public partial class Settings : SettingsController
    {
        public override string HeaderText
        {
            get
            {
                return "პარამეტრები";
            }
        }

        public override ScrollBarVisibility ScrollBarVis
        {
            get
            {
                return ScrollBarVisibility.Disabled;
            }
        }

        public Settings()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            CurrentModel.Refresh();
        }

        public override bool ShowHomeBtn
        {
            get
            {
                return true;
            }
        }

        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.Settings;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            CurrentModel.Save();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CurrentModel.Cancel();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "xml files (*.xml)|*.xml";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                //if ((myStream = saveFileDialog1.OpenFile()) != null)
                //{
                //    // Code to write the stream goes here.
                //    myStream.Close();
                //}
                XmlSerializer SerializerObj = new XmlSerializer(typeof(Setting));

                // Create a new file stream to write the serialized object to a file
                using (TextWriter WriteFileStream = new StreamWriter(saveFileDialog1.FileName))
                {
                    SerializerObj.Serialize(WriteFileStream, SettingsManager.Current.GetSettings());
                    MessageBox.Show("ექსპორტი დასრულდა წარმატებით");
                }
            }



        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            openFileDialog.Filter = "xml files (*.xml)|*.xml";
            openFileDialog.RestoreDirectory = true;

            XmlSerializer serializer = new XmlSerializer(typeof(Setting));
            if(openFileDialog.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    Setting st = (Setting)serializer.Deserialize(fs);
                    CurrentModel.Import(st);
                    MessageBox.Show("იმპორტი დასრულდა წარმატებით");
                }
            }
        }
    }
}
