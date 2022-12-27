using Azure.Storage;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZENC.SAMPLE.OFFICE.OUTLOOK
{
    internal class TempClass
    {
        public static BlobServiceClient GetBlobServiceClient(string accountName, string accountKey)
        {
            Azure.Storage.StorageSharedKeyCredential sharedKeyCredential =
                new StorageSharedKeyCredential(accountName, accountKey);

            string blobUri = "https://" + accountName + ".blob.core.windows.net";



            return new BlobServiceClient
                (new Uri(blobUri), sharedKeyCredential);
        }


        public static string UploadBlob(string acc, string accKey, string containerName, string uploadPath,string id)
        {
            BlobServiceClient serviceClient = GetBlobServiceClient(acc, accKey);

            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);

            string name = Path.Combine(id, System.IO.Path.GetFileName(uploadPath));
            BlobClient blobClient = containerClient.GetBlobClient(name);

            using (FileStream uploadFileStream = File.OpenRead(uploadPath))
            {
                blobClient.Upload(uploadFileStream, true);
            }

            BlobClient d = containerClient.GetBlobClient(name);
            
            return d.Uri.AbsoluteUri;
        }


       
    }
}
