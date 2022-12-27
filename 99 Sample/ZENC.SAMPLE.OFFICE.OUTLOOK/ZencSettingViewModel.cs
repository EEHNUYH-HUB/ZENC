using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZENC.CORE.Util.NotionApi;
using ZENC.CORE.WPF;

namespace ZENC.SAMPLE.OFFICE.OUTLOOK
{
    public class ZencSettingViewModel : ZENC.CORE.WPF.EzChangedNotificator
    {
       

        public EzCommand SaveCommand { get; set; }

        string apiKey = string.Empty;
        public string ApiKey { get { return apiKey; } set { apiKey = value; OnPropertyChanged("ApiKey"); } }
        string databaseID = string.Empty;
        public string DatabaseID { get { return databaseID; } set { databaseID = value; OnPropertyChanged("DatabaseID"); } }

        Window currentWindow;
        public ZencSettingViewModel(Window window)
        {
            window.DataContext = this;
            currentWindow = window;
            SaveCommand = new EzCommand();

            ApiKey = Properties.Settings.Default.apikey;
            DatabaseID = Properties.Settings.Default.database;

            SaveCommand.ExecuteDelegate = (p) =>
            {

                Properties.Settings.Default.apikey = ApiKey;
                Properties.Settings.Default.database = DatabaseID;
                Properties.Settings.Default.Save();

                currentWindow.Close();
            };
          
        }


    }
}
