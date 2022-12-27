using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE;
namespace ZENC.AZURE.Storage.Blobs
{
    public class BlobManager
    {
        public BlobServiceClient ServiceClient { get; set; }

        public BlobManager(string accountName, string accountKey) : this(GetBlobServiceClient(accountName, accountKey)) { }
        public BlobManager(BlobServiceClient serviceClient) { ServiceClient = serviceClient; }

        public async Task<BlobClient> UploadFileAsync(string containerName, string folderName, string uploadPath)
        {
            BlobContainerClient containerClient = ServiceClient.GetBlobContainerClient(containerName);

            string fileName = uploadPath.EzFileName();
            fileName = folderName.EzCombine(fileName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            using (FileStream uploadFileStream = File.OpenRead(uploadPath))
            {
                await blobClient.UploadAsync(uploadFileStream, true);
            }

            return blobClient;
        }

        public List<string> GetContainers()
        {
            List<string> rtn = new List<string>();
            Pageable<BlobContainerItem> lst = ServiceClient.GetBlobContainers();
            foreach (var item in lst)
            {
                if (item.EzNotNull() && (item.IsDeleted.EzIsNull() || !(bool)item.IsDeleted))
                    rtn.Add(item.Name);
            }

            return rtn;
        }

        public List<string> GetFiles(string containerName) 
        {
            List<string> rtn = new List<string>();
            BlobContainerClient containerClient = ServiceClient.GetBlobContainerClient(containerName);
            Pageable<BlobItem> lst = containerClient.GetBlobs();
            foreach (var item in lst)
            {
                if (item.EzNotNull()  && !item.Deleted)
                    rtn.Add(item.Name);
            }
            return rtn;
        }
        public static BlobServiceClient GetBlobServiceClient(string accountName, string accountKey)
        {
            Azure.Storage.StorageSharedKeyCredential sharedKeyCredential =
                new StorageSharedKeyCredential(accountName, accountKey);

            string blobUri = "https://" + accountName + ".blob.core.windows.net";

            return new BlobServiceClient(new Uri(blobUri), sharedKeyCredential);
        }
    }
}
