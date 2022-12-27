using EEH.HTML;
using Microsoft.Office.Interop.Outlook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZENC.CORE.Util.NotionApi;
using ZENC.CORE.WPF;

namespace ZENC.SAMPLE.OFFICE.OUTLOOK
{
    public class ZencUploaderViewModel : EzChangedNotificator
    {
        public class PropertyOption
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
        }

        public class PropertyModel : EzChangedNotificator
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string Type { get; set; }

            public List<PropertyOption> Items { get; set; }

            PropertyOption selectedItem;
            public PropertyOption SelectedItem { get { return selectedItem; } set { selectedItem = value; OnPropertyChanged("SelectedItem"); } }
        }

        ObservableCollection<PropertyModel> items;
        public ObservableCollection<PropertyModel> Items { get { return items; } set { items = value; OnPropertyChanged("Items"); } }

        string apiKey = string.Empty;
        public string ApiKey { get { return apiKey; } set { apiKey = value; OnPropertyChanged("ApiKey"); } }
        string databaseID = string.Empty;
        public string DatabaseID { get { return databaseID; } set { databaseID = value; OnPropertyChanged("DatabaseID"); } }


        string progressText = "진행 상태를 표시 합니다.";
        public string ProgressText { get { return progressText; } set { progressText = value; OnPropertyChanged("ProgressText"); } }

        int progressValue;
        public int ProgressValue { get { return progressValue; } set { progressValue = value; OnPropertyChanged("ProgressValue"); } }


        Window currentWindow;


        public EzCommand RunCommand { get; set; }
        MailItem selectedItem;
        bool isAll;
        public ZencUploaderViewModel(Window window, MailItem selectedItem, bool isAll)
        {
            window.DataContext = this;
            currentWindow = window;
            this.isAll = isAll;
            this.selectedItem = selectedItem;
            RunCommand = new EzCommand();
            RunCommand.ExecuteDelegate = (p) =>
            {
                Run();
            };

            ApiKey = Properties.Settings.Default.apikey;
            DatabaseID = Properties.Settings.Default.database;

            Load();



        }
        string accID = "notionstor";//notionblobtest
        string accKey = "r8o5BAhNhyB5y75/jW2wcWbqVO2jcUkN/ILJNozTawLgGmV2f24aegE+p4LtuDBFyku2ekNb3TUV+AStELRQgw==";//D7d0XS6GCZ70DKLtustRxsJWPEjxhxnVbHB2Mls4kDqPAx2etR+Vnf7sgQm6SNrWTgTjxxBdb2WY+AStLmRHoQ==
        string container = "data";//img
        public bool Run()
        {
           
                if (!string.IsNullOrEmpty(ApiKey) &&
                    !string.IsNullOrEmpty(DatabaseID))
                {
                    EzRunAsyncAndCompleteSyncTasks.Instance.AddTask( () =>
                    {
                        NotionApiClient notionClient = new NotionApiClient(ApiKey);

                        NotionBlocks blocks = new NotionBlocks();
                        blocks.AddHeader(selectedItem.SenderName, 3);


                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(string.Format("받은시간 : {0}", selectedItem.ReceivedTime));


                        if (selectedItem.Recipients != null)
                        {
                            StringBuilder sb2 = new StringBuilder();
                            sb2.Append("받는사람 : ");
                            foreach (Recipient recipient in selectedItem.Recipients)
                            {
                                sb2.Append(recipient.Name + ";");
                            }
                            sb.AppendLine(sb2.ToString());
                        }
                        sb.AppendLine(" ");
                        //sb.AppendLine(selectedItem.Body);
                        blocks.AddContent(sb.ToString(), true);

                        string id = Guid.NewGuid().ToString();

                        BindingProgressValue(10, "원본메일 업로드중 입니다.");
                        List<ExpandoObject> ImageList = new List<ExpandoObject>();
                        List<int> indexList = new List<int>();
                        string tempFolder = System.IO.Path.Combine(Environment.GetEnvironmentVariable("temp"), Guid.NewGuid().ToString());
                        DirectoryInfo info = new DirectoryInfo(tempFolder);
                        info.Create();

                        blocks.AddContent("원본메일.msg", true);
                        string orgMsgPath = System.IO.Path.Combine(tempFolder, "원본메일.msg");
                        selectedItem.SaveAs(orgMsgPath);
                        string url =  TempClass.UploadBlob(accID, accKey, container, orgMsgPath,id);
                        blocks.AddFile(url, "file");
                        File.Delete(orgMsgPath);
                        BindingProgressValue(20, "첨부 파일 업로드중 입니다.");
                        if (selectedItem.Attachments != null && selectedItem.Attachments.Count > 0)
                        {

                            foreach (Attachment item in selectedItem.Attachments)
                            {
                                string savePath = System.IO.Path.Combine(tempFolder, item.FileName);
                                item.SaveAsFile(savePath);
                                url =  TempClass.UploadBlob(accID, accKey, container, savePath,id);

                                string name = item.FileName.Replace(" ", "_");

                                string ext = System.IO.Path.GetExtension(item.FileName);

                                int index = selectedItem.HTMLBody.IndexOf("cid:" + item.FileName);
                                if (index > -1)
                                {
                                    dynamic dy = new ExpandoObject();
                                    dy.index = index;
                                    dy.url = url;
                                    dy.name = item.FileName;
                                    ImageList.Add(dy);

                                }
                                else
                                {
                                    blocks.AddContent(item.FileName, true);
                                    blocks.AddFile(url, "file");
                                }
                                

                                File.Delete(savePath);


                            }
                        }

                        Directory.Delete(tempFolder);

                        BindingProgressValue(50, "메일 내용 분석중 입니다.");
                        EEH.HTML.HtmlDocument document = new EEH.HTML.HtmlDocument();
                        document.LoadHtml(selectedItem.HTMLBody);

                        List<ExpandoObject> nodeList = new List<ExpandoObject>();
                        int startIndex = 0;
                        BindingTextAndImage(document.DocumentNode, nodeList, ImageList, selectedItem.Body, ref startIndex);

                        sb.Clear();
                        startIndex = 0;
                        bool isStop = false;

                        foreach (dynamic node in nodeList)
                        {

                            if (blocks.AddContent(selectedItem.Body.Substring(startIndex, node.startIndex - startIndex), isAll))
                            {

                                startIndex = node.startIndex;
                                blocks.AddFile(node.text, "image");
                            }
                            else
                            {
                                isStop = true;
                                break;
                            }


                        }

                        if (!isStop)
                        {
                            if (startIndex < selectedItem.Body.Length)
                            {
                                blocks.AddContent(selectedItem.Body.Substring(startIndex, selectedItem.Body.Length - startIndex), isAll);

                            }
                        }
                        BindingProgressValue(70, "노션에 업로드중 입니다.");

                        Dictionary<string, object> dic = new Dictionary<string, object>();

                        foreach (var item in Items)
                        {
                            if (item.SelectedItem != null)
                            {
                                JObject jobj = new JObject();
                                JObject jV = new JObject();
                                jV["name"] = item.SelectedItem.Name;
                                jobj["select"] = jV;
                                dic.Add(item.Name, jobj);
                            }
                        }

                        Properties.Settings.Default.lastoption = JsonConvert.SerializeObject(dic);
                        Properties.Settings.Default.Save();

                        string pageID = notionClient.CreatePage(DatabaseID, selectedItem.Subject, dic, blocks);


                        BindingProgressValue(100, "완료 되었습니다.");


                    }, () =>
                    {
                        currentWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new System.Action(delegate
                        {
                            currentWindow.Close();
                        }));
                    },(System.Exception ex) => {

                        currentWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new System.Action(delegate
                        {
                            MessageBox.Show(ex.Message);
                            currentWindow.Close();
                        }));
                    });

                    return true;
                }
                else
                {
                    MessageBox.Show("설정 정보가 정확하지 않습니다.");
                    currentWindow.Close();
                    return false;
                }
          
        }

        void BindingProgressValue(int prgValue, string text)
        {
            currentWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new System.Action(delegate
            {
                ProgressText = text;
                ProgressValue = prgValue;
            }));
        }



        void BindingTextAndImage(HtmlNode node, List<ExpandoObject> rtn, List<ExpandoObject> urls, string content, ref int startIndex)
        {

            if (node != null)
            {
                if (node.NodeType == HtmlNodeType.Text)
                {
                    if (node.ParentNode == null || (node.ParentNode != null && node.ParentNode.Name != "style"))
                    {
                        string txt = node.InnerText.Replace("&nbsp;", "");

                        if (!string.IsNullOrEmpty(txt))
                        {
                            string checkStr = content.Substring(startIndex, content.Length - startIndex);

                            int index = checkStr.IndexOf(txt);

                            if (index != -1)
                            {
                                startIndex = startIndex + index + txt.Length;
                            }
                        }
                        //rtn.Add(dy);
                    }
                }
                else if (node.Name == "img")
                {
                    if (node.OuterHtml.Contains("cid:"))
                    {
                        string outPutText = node.OuterHtml;
                        foreach (dynamic obj in urls)
                        {
                            if (outPutText.Contains(obj.name))
                            {
                                dynamic dy = new ExpandoObject();
                                dy.type = "img";
                                dy.text = obj.url;
                                dy.startIndex = startIndex;
                                rtn.Add(dy);
                                break;
                            }
                        }
                    }
                    else
                    {

                    }

                }



                foreach (HtmlNode child in node.ChildNodes)
                {
                    BindingTextAndImage(child, rtn, urls, content, ref startIndex);
                }
            }
        }
        void Load()
        {
            NotionApiClient client = new NotionApiClient(ApiKey);
            JObject r = client.GetDatabase(DatabaseID);
            Items = new ObservableCollection<PropertyModel>();

            if (r != null)
            {
                var iter = r.GetEnumerator();
                while (iter.MoveNext())
                {
                    var keyValue = iter.Current;
                    string key = keyValue.Key;



                    if (key == "properties")
                    {
                        JObject value = keyValue.Value as JObject;
                        if (value != null)
                        {
                            var propertyEum = value.GetEnumerator();
                            while (propertyEum.MoveNext())
                            {
                                var propertyKeyValue = propertyEum.Current;
                                var propertyName = propertyKeyValue.Key;
                                JObject jobj = propertyKeyValue.Value as JObject;
                                if (jobj != null)
                                {

                                    PropertyModel model = new PropertyModel();
                                    model.ID = jobj["id"].ToString();
                                    model.Type = jobj["type"].ToString();
                                    model.Name = jobj["name"].ToString();


                                    if (model.Type == "select")
                                    {
                                        model.Items = new List<PropertyOption>();
                                        JObject selectJObject = jobj[model.Type] as JObject;
                                        JArray optionJObject = selectJObject["options"] as JArray;
                                        foreach (var obj in optionJObject)
                                        {
                                            model.Items.Add(new PropertyOption { ID = obj["id"].ToString(), Name = obj["name"].ToString(), Color = obj["color"].ToString() });

                                        }
                                        Items.Add(model);
                                    }
                                }
                            }

                        }
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(Properties.Settings.Default.lastoption))
                {
                    Dictionary<string, JObject> dic = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(Properties.Settings.Default.lastoption);


                    foreach (var keyValue in dic)
                    {
                        List<PropertyModel> finds = Items.Where(x => x.Name == keyValue.Key).ToList();
                        if (finds.Count > 0)
                        {
                            var item = finds[0];

                            if (item != null && item.Items != null && keyValue.Value["select"] != null)
                            {
                                JObject selectObj = keyValue.Value["select"] as JObject;
                                if (selectObj != null)
                                {
                                    string v = selectObj["name"].ToString();
                                    var subs = item.Items.Where(x => x.Name == v).ToList();
                                    if(subs!= null && subs.Count > 0)
                                    {
                                        item.SelectedItem = subs[0];
                                    }
                                }
                            }
                        }
                    }
                }


            }
        }
    }
}
