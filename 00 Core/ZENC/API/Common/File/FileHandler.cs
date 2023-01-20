﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using ZENC.CORE;
using ZENC.CORE.API.Common.File;
using ZENC.CORE.API.Common.File.Entity;
using ZENC.CORE.API.Parameters;


namespace ZENC.SAMPLE.BIZ
{
    public class FileHandler : IFileHandler
    {

        public FileHandler(string path)
        {
            RootFolder = path.ExCreateDirectory();
        }
        public string RootFolder { get; set; }
        public const long BUFFERSIZE = 2048000;
        public long BufferSize { get { return BUFFERSIZE; } }



        public DownloadResult Read(string staticID)
        {
            string folderPath = RootFolder.ExCombine(staticID);
            if (folderPath.ExIsExists())
            {
                DirectoryInfo dir = new DirectoryInfo(folderPath);

                if (dir.Exists)
                {
                    List<FileInfo> files = dir.FullName.ExFileList();
                    if (files.ExNotNull() && files.Count > 0)
                    {
                        DownloadResult result = new DownloadResult();
                        result.DownloadStream = new FileStream(files[0].FullName, FileMode.Open, FileAccess.Read);
                        result.FileName = files[0].Name;
                        return result;
                    }
                }
            }

            return null;
        }


        public UploadResult Write(FileParameter param)
        {
            try
            {

                if (param.ExNotNull())
                {
                    UploadResult result = new UploadResult();

                    result.ResultType = UploadResultType.PROGRESS;
                    if (param.ExNotNull())
                    {
                        string name = param.FileName.ExToLower();
                        long totalSize = param.FileSize.ExLong();
                        byte[] buffer = param.Base64String.ExBase64Byte();
                        if (buffer.ExNotNull())
                        {
                            string folder = RootFolder.ExCombine(param.StaticID).ExCreateDirectory();
                            using (FileStream fs = new FileStream(folder.ExCombine(name), FileMode.Append, FileAccess.Write))
                            {
                                long currentSize = fs.Length + buffer.Length;
                                fs.Write(buffer, 0, buffer.Length);

                                if (currentSize < totalSize)
                                {
                                    result.ResultType = UploadResultType.PROGRESS;
                                }
                                else
                                {
                                    result.ResultType = UploadResultType.COMPLETED;
                                }
                            }
                        }

                    }

                    return result;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                UploadResult result = new UploadResult();
                result.ResultType = UploadResultType.ERROR;
                result.ErrorMsg = ex.Message;
                return result;
            }
        }





    }
}
