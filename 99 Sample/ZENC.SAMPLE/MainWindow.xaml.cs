using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using ZENC.CORE.Cryptography;
using ZENC.CORE;
using System.Windows.Markup.Localizer;
using System.IO;
using ZENC.CORE.API.Common.File.Entity;
using ZENC.CORE.Util;
using ZENC.CORE.API.Common.File;
using ZENC.CORE.Util.NotionApi;

using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.ComponentModel;
using ZENC.AZURE.Storage.Blobs;
using System.Windows.Interop;
using ZENC.AZURE.Resources;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic.FileIO;

namespace ZENC.SAMPLE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string databaseid = "a9d4ae312b414b9ba7851e19d76fd1f2";

            NotionApiClient client = new NotionApiClient("secret_6IKRSDKGoYuyyFa8LYzHOo6jzlaQp2dpvVEGdYAGnYc");

            string result = client.CreatePage(databaseid, "TEST Page", null, null);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                BlobManager mng = new BlobManager("notionblobtest", "D7d0XS6GCZ70DKLtustRxsJWPEjxhxnVbHB2Mls4kDqPAx2etR+Vnf7sgQm6SNrWTgTjxxBdb2WY+AStLmRHoQ==");
                BlobClient c= await mng.UploadFileAsync("img","test", ofd.FileName);


                mng.GetContainers();
                mng.GetFiles("img");
            }

        }

       

        private static string GetAccountSASToken(StorageSharedKeyCredential key)
        {
            // Create a SAS token that's valid for one hour.
            AccountSasBuilder sasBuilder = new AccountSasBuilder()
            {
                Services = AccountSasServices.Blobs | AccountSasServices.Files,
                ResourceTypes = AccountSasResourceTypes.Service,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                Protocol = SasProtocol.Https
            };

            sasBuilder.SetPermissions(AccountSasPermissions.Read |
                AccountSasPermissions.Write);

            // Use the key to get the SAS token.
            string sasToken = sasBuilder.ToSasQueryParameters(key).ToString();

            Console.WriteLine("SAS token for the storage account is: {0}", sasToken);
            Console.WriteLine();

            return sasToken;
        }


        private void Get(string accountID, string accountKey, string containerName, string blobName)
        {
            CloudStorageAccount storageAccount
             = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net", accountID, accountKey));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);



            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);




        }


        public static BlobServiceClient GetBlobServiceClient(string accountName, string accountKey)
        {
            Azure.Storage.StorageSharedKeyCredential sharedKeyCredential =
                new StorageSharedKeyCredential(accountName, accountKey);

            string blobUri = "https://" + accountName + ".blob.core.windows.net";



            return new BlobServiceClient
                (new Uri(blobUri), sharedKeyCredential);
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string clientId = "74a7177a-327b-43de-b8e9-713df68777da";
            string tenantId = "785087ba-1e72-4e7d-b1d1-4a9639137a66";
            string[] scopes = { "user.read" };

            var app = PublicClientApplicationBuilder.Create(clientId)
                    .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                    .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                    .Build();
            

            var result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            string s = result.AccessToken;


            //ZENC.AZURE.Auth.OAuth2AzureAD a = new AZURE.Auth.OAuth2AzureAD();
            //a.GenerateKey(null);

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //var reader = new StreamReader(@"C:\\Users\\hhlee\\AppData\\Roaming\\USAGEDTL\\88649182\\20230301-20230331\\88649182_621726d5-16ef-4f88-9f2b-1e3ce425a645.csv");
            //var csv = new CsvReader(reader,System.Globalization.CultureInfo.CurrentCulture);


            string coloumn1;
            string coloumn2;

            //C:\Users\hhlee\AppData\Roaming\USAGEDTL/88649182/20230301-20230331/88649182_621726d5-16ef-4f88-9f2b-1e3ce425a645.csv
            var path = @"C:\\Users\\hhlee\\AppData\\Roaming\\USAGEDTL\\88649182\\20230301-20230331\\88649182_621726d5-16ef-4f88-9f2b-1e3ce425a645.csv";
            using (TextFieldParser csvReader = new TextFieldParser(path))
            {
                csvReader.CommentTokens = new string[] { "#" };
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;

                

                while (!csvReader.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvReader.ReadFields();
                    coloumn1 = fields[0];
                    coloumn2 = fields[1];
                }

            }

            string t = "STIS,STIS,jspark@stis.co.kr,5e17863a-6160-4e49-a091-62279a5ab5c2,Microsoft Azure 엔터프라이즈,03-Networks,All Regions,03/05/2023,Azure Front Door Service - Standard Default Ruleset,Azure Front Door Service,,624658fc-567a-41bb-bc91-faea35599c36,Standard Default Ruleset,,1/Month,0.001344086021505,22476.793388429691,30.210743801652892,EC-YK,Microsoft.Network,/subscriptions/5e17863a-6160-4e49-a091-62279a5ab5c2/resourceGroups/03-Networks/providers/Microsoft.Network/frontdoorwebapplicationfirewallpolicies/oneulpluswafpolicy,,MS-AZR-0017P,\"{  \"\"Provider\"\": \"\"3\"\",  \"\"ConsumptionBeginTime\"\": \"\"2023-03-05T00:00:00Z\"\",  \"\"ConsumptionEndTime\"\": \"\"2023-03-05T00:59:00Z\"\"}\",,,oneulpluswafpolicy,,,22498,,,,Azure,,Usage,UsageBased,OnDemand,,61180007,\"Syntek Information Systems Co.,Ltd.\",KRW,03/01/2023,03/31/2023,61180007,\"Syntek Information Systems Co.,Ltd.\",,True,AAD-56826,22498.0,,Compute,,,";

            List<string> strs = Utils.GetSplitUsingCSV(t);
        }
       
    }


    public class Utils
    {
        public static List<string> GetSplitUsingCSV(string str)
        {
            int len = str.IndexOf(",\"");
            int len2 = str.IndexOf("\"{");

            if (len > -1)
            {
                List<string> strs = new List<string>();

                string str1 = str.Substring(0, len);
                strs.AddRange(str1.Split(new string[] { "," }, StringSplitOptions.TrimEntries).ToList());
                len += 1;
                string str2 = str.Substring(len, str.Length - len);

                len = str2.IndexOf("\",");
                len += 1;
                string str3 = str2.Substring(0, len);

                strs.Add(str3);
                len += 1;
                string str4 = str2.Substring(len, str2.Length - len);
                strs.AddRange(GetSplitUsingCSV(str4));

                return strs;
            }
            else
            {
                return str.Split(new string[] { "," }, StringSplitOptions.TrimEntries).ToList();
            }
        }

    }
}

