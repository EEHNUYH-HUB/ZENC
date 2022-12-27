using EEH.HTML;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
using ZENC.CORE.Util.NotionApi;

namespace ZENC.SAMPLE.OFFICE.OUTLOOK
{
    /// <summary>
    /// ZencUploader.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ZencUploader : Window
    {
        ZencUploaderViewModel viewModel;
        public ZencUploader(MailItem selectedItem,bool isAll)
        {
            InitializeComponent();
            viewModel = new ZencUploaderViewModel(this, selectedItem,isAll);

        }

       
        
    }
}
