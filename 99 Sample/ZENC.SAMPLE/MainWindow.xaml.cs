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

        }
       
    }
}

