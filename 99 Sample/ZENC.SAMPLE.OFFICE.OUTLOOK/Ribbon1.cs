using EEH.HTML;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace ZENC.SAMPLE.OFFICE.OUTLOOK
{
    public partial class Ribbon1
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.apikey))
            {
                Properties.Settings.Default.apikey = "secret_2vTgrB33Ml1tDoIzMHJ0av60E2eBbkFRQrEcwLybnbV";
                Properties.Settings.Default.database = "ae0980e5f4a94e6f99f8d7d3a3a08213";
                Properties.Settings.Default.Save();
            }
            
            ThisAddIn.CurrentInstance.ItemLoad += new Microsoft.Office.Interop.Outlook.ApplicationEvents_11_ItemLoadEventHandler(Application_ItemLoad);
            this.btnAll.Click += BtnAll_Click;
            this.btnLast.Click += BtnLast_Click;
            this.btnSetting.Click += BtnSetting_Click;
        }

        private void BtnAll_Click(object sender, RibbonControlEventArgs e)
        {
            if (selectedItem != null)
            {
                ZencUploader uploader = new ZencUploader(selectedItem, true);

                uploader.ShowDialog();
                selectedItem = null;
            }
            else
            {
                MessageBox.Show("메일을 선택해주세요.");
            }
        }

        private void BtnSetting_Click(object sender, RibbonControlEventArgs e)
        {
            ZencSetting setting = new ZencSetting();
            setting.ShowDialog();
        }

        Outlook.MailItem selectedItem;
        private void BtnLast_Click(object sender, RibbonControlEventArgs e)
        {
            if (selectedItem != null)
            {
                ZencUploader uploader = new ZencUploader(selectedItem, false);

                uploader.ShowDialog();

                selectedItem = null;
            }
            else
            {
                MessageBox.Show("메일을 선택해주세요.");
            }

        }


        void Application_ItemLoad(object Item)
        {
            selectedItem = Item as Outlook.MailItem;
        }
        private void MailItem_Open(ref bool Cancel)
        {
            Outlook.Inspector inspector = ThisAddIn.CurrentInstance.ActiveInspector();
            Outlook.Explorer explorer = ThisAddIn.CurrentInstance.ActiveExplorer();

            object tmpObject = null;



            try
            {
                if (inspector != null
                    && inspector.CurrentItem != null)
                    tmpObject = inspector.CurrentItem;

                if (explorer != null
                    && explorer.Selection != null
                    && explorer.Selection.Count > 0)
                    tmpObject = explorer.Selection[1];



                selectedItem = tmpObject as Outlook.MailItem;
            }
            catch
            {

            }
        }





    }
}
