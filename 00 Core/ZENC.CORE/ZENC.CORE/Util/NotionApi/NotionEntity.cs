using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ZENC.CORE.Util.NotionApi
{

    public class NotionPage
    {
        [JsonProperty("parent")]
        public NotionDatabase Parent { get; set; }
        [JsonProperty("properties")]
        public dynamic Properties { get; set; }
        [JsonProperty("children")]
        public List<ExpandoObject> Children { get; set; }

        public static NotionPage GetInstance(string databaseID, string title, Dictionary<string, object> properties, NotionBlocks blocks)
        {
            NotionPage page = new NotionPage();
            page.Parent = new NotionDatabase();
            page.Parent.DatabaseID = databaseID;
            page.Properties = new ExpandoObject();
            page.Properties.Name = new NotionTitleTexts();
            page.Properties.Name.Title = new List<NotionText>();
            page.Properties.Name.Title.Add(new NotionText { Text = new NotionContent { Content = title } });

            var dic = page.Properties as IDictionary<string, object>;

            foreach (var keyValue in properties)
            {
                dic.Add(keyValue.Key, keyValue.Value);
            }
            if (blocks != null)
                page.Children = blocks.Children;
            return page;
        }
    }

    public class NotionDatabase
    {
        [JsonProperty("database_id")]
        public string DatabaseID { get; set; }
    }

    public class NotionID
    {
        public string id { get; set; }
    }
    public class NotionBlocks
    {
        [JsonProperty("children")]
        public List<ExpandoObject> Children { get; set; }
        public NotionBlocks()
        {
            Children = new List<ExpandoObject>();
        }



        public void AddHeader(string str, int size)
        {
            dynamic item = new ExpandoObject();

            item.type = "heading_" + size.ToString();
            var dic = item as IDictionary<string, object>;
            dic.Add("object", "block");
            NotionRichTexts obj = new NotionRichTexts();
            obj.RichText = new List<NotionText>();
            NotionText txt = new NotionText();

            txt.Text = new NotionContent();
            txt.Text.Content = str;
            //obj.RichText.AddRange(SplitContent(txt, 50));
            obj.RichText.Add(txt);
            dic.Add(item.type, obj);

            Children.Add(item);
        }
        public bool AddContent(string str, bool isAll)
        {

            bool rtn = true;
            List<NotionRichTexts> objs = CreateNotionRichTextObj(str, isAll,90,out rtn);
            foreach (var obj in objs)
            {
                dynamic item = new ExpandoObject();

                item.type = "paragraph";
                var dic = item as IDictionary<string, object>;
                dic.Add("object", "block");
                dic.Add(item.type, obj);

                Children.Add(item);
            }
            return rtn;
        }


      
        private static List<NotionRichTexts> CreateNotionRichTextObj(string str, bool isAll,int maxLen,out bool isRun)
        {
            List<NotionRichTexts> rtn = new List<NotionRichTexts>();
            NotionRichTexts obj = new NotionRichTexts();
            obj.RichText = new List<NotionText>();
            rtn.Add(obj);
            string[] strs = str.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            bool isFirst = true;

            List<NotionText> tmpList = new List<NotionText>();
            bool isFrom = false;
            bool isSend = false;
            //bool isSubject = false;
            bool isTo = false;
            isRun = true;
            foreach (var s in strs)
            {
                if (isFirst && s != null && s.Trim().Length == 0)
                {
                    continue;
                }
                NotionText txt = new NotionText();
                txt.Text = new NotionContent();
                txt.Text.Content = (isFirst ? "" : "\r\n") + s.TrimStart();

                if (!isAll)
                {
                    if (s.Contains("From"))
                    {
                        isFrom = true;
                        tmpList.Add(txt);
                    }
                    else if (s.Contains("Sent"))
                    {
                        isSend = true;
                        tmpList.Add(txt);
                    }
                    else if (s.Contains("To"))
                    {
                        isTo = true;
                        tmpList.Add(txt);
                    }
                    //else if (s.Contains("Subject"))
                    //{
                    //    isSubject = true;
                    //    tmpList.Add(txt);
                    //}
                    else if (!isFrom && !isSend && !isTo)
                    {
                        //obj.RichText.AddRange(SplitContent(txt, 50));
                        if(obj.RichText.Count > maxLen)
                        {
                            obj = new NotionRichTexts();
                            obj.RichText = new List<NotionText>();
                            rtn.Add(obj);
                        }
                        obj.RichText.Add(txt);
                    }

                    else
                    {
                        isFrom = isSend =  isTo = false;
                        foreach (var t in tmpList)
                        {
                            if (obj.RichText.Count > maxLen)
                            {
                                obj = new NotionRichTexts();
                                obj.RichText = new List<NotionText>();
                                rtn.Add(obj);
                            }
                            obj.RichText.Add(txt);

                        }
                        tmpList.Clear();
                    }
                    isFirst = false;
                    if (isFrom && isSend &&  isTo)
                    {
                        isRun = false;
                        break;
                    }
                }
                else
                {
                    if (obj.RichText.Count > maxLen)
                    {
                        obj = new NotionRichTexts();
                        obj.RichText = new List<NotionText>();
                        rtn.Add(obj);
                    }
                    obj.RichText.Add(txt);
                }

            }

            return rtn;
        }

        List<NotionText> SplitContent(NotionText txt, int len)
        {
            string str = txt.Text.Content;

            int totalLen = str.Length;

            int cnt = totalLen / len;

            cnt += 1;
            List<NotionText> rtn = new List<NotionText>();
            for (int i = 0; i < cnt; i++)
            {
                int startIndex = i * len;
                if ((startIndex + len) < totalLen)
                {
                    string newstr = str.Substring(startIndex, len);
                    NotionText ntxt = new NotionText();
                    ntxt.Text = new NotionContent();
                    ntxt.Text.Content = newstr;
                    rtn.Add(ntxt);

                }
                else
                {
                    int newLen = totalLen - (startIndex);
                    if (newLen > 0)
                    {
                        string newstr = str.Substring(startIndex, newLen);
                        NotionText ntxt = new NotionText();
                        ntxt.Text = new NotionContent();
                        ntxt.Text.Content = newstr;
                        rtn.Add(ntxt);
                    }
                }
            }

            return rtn;


        }
        public void AddFile(string url, string type)
        {
            dynamic item = new ExpandoObject();

            item.type = type;
            var dic = item as IDictionary<string, object>;
            dic.Add("object", "block");
            NotionImageBlock obj = new NotionImageBlock();
            obj.BlockType = "external";
            obj.External = new NotionUrl();
            obj.External.Url = url;

            dic.Add(item.type, obj);
            Children.Add(item);
        }
    }


    public class NotionImageBlock
    {
        [JsonProperty("type")]
        public string BlockType { get; set; }
        [JsonProperty("external")]
        public NotionUrl External { get; set; }
    }

    public class NotionUrl
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class NotionTitleTexts
    {
        [JsonProperty("title")]
        public List<NotionText> Title { get; set; }
    }
    public class NotionRichTexts
    {
        [JsonProperty("rich_text")]
        public List<NotionText> RichText { get; set; }
    }
    public class NotionText
    {
        [JsonProperty("text")]
        public NotionContent Text { get; set; }
    }
    public class NotionContent
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
