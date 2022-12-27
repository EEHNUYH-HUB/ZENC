using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE.API.Client;

namespace ZENC.CORE.Util.NotionApi
{
    public class NotionApiClient
    {
        RestAPIHandler apiHandler;

        public NotionApiClient(string apikey,string baseUrl= "https://api.notion.com/v1/", string version = "2022-06-28") {
            apiHandler = new RestAPIHandler(baseUrl, (httpClient) =>
            {
                httpClient.DefaultRequestHeaders.Add("Notion-Version", version);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apikey);
            });
        }
        public string CreatePage(NotionPage page)
        {
            NotionID result = apiHandler.Post<NotionID>("pages", page);
            return result.id;
        }
        public string CreatePage(string databaseID,string title, Dictionary<string,object> properties, NotionBlocks blocks)
        {
            return CreatePage(NotionPage.GetInstance(databaseID, title, properties, blocks));
        }

        public string CreateBlocks(string databaseID, NotionBlocks blocks)
        {
            string result = apiHandler.Post(string.Format("blocks/{0}/children", databaseID), blocks);
            return result;
        }


        public JObject GetDatabase(string databaseID)
        {
            JObject result = apiHandler.Get<JObject>(string.Format("databases/{0}", databaseID));
            return result;
        }

    }
}
