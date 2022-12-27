using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EEH.HTML;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Dynamic;
using ZENC.CORE.API.Common.File.Entity;
using System.Drawing;
using System.Security.Policy;
using System.IO;

namespace ZENC.SAMPLE.OFFICE.OUTLOOK
{
    public partial class ThisAddIn
    {
        public static Microsoft.Office.Interop.Outlook.Application CurrentInstance { get; set; }
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            CurrentInstance = Application;
        }

      
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}

