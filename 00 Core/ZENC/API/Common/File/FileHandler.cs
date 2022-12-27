
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using ZENC.CORE;
using ZENC.CORE.API.Common.File.Entity;
using ZENC.CORE.API.Parameters;


namespace ZENC.SAMPLE.BIZ
{
    public class FileHandler : ZENC.CORE.API.Common.File.IFileHandler
    {

        public FileHandler(string path)
        {
            RootFolder = path.EzCreateDirectory();
        }
        public string RootFolder { get; set; }
        public const long BUFFERSIZE =  2048000 ;
        public long BufferSize { get { return BUFFERSIZE; } }

       

        public DownloadResult Read(string strID, string number)
        {
            string folderPath = RootFolder.EzCombine(strID);
            if (folderPath.EzIsExists())
            {
                List<DirectoryInfo> dirs = folderPath.EzDirectoryList();

                if (dirs.EzNotNull())
                {
                    foreach (var dir in dirs)
                    {
                        if (dir.Name.EzToLower() == number)
                        {
                            List<FileInfo> files = dir.FullName.EzFileList();
                            if (files.EzNotNull() && files.Count > 0)
                            {
                                DownloadResult result = new DownloadResult();
                                result.DownloadStream = new FileStream(files[0].FullName, FileMode.Open, FileAccess.Read);
                                result.FileName = files[0].Name;
                                return result;
                            }
                        }
                    }
                }
            }

            return null;
        }

        
        public UploadResult Write(FileParameter param)
        {
            try
            {
                if (param.EzNotNull())
                {
                    UploadResult result = new UploadResult();

                    result.ResultType = UploadResultType.PROGRESS;
                    if (param.EzNotNull())
                    {
                        string name = param.FileName.EzToLower();
                        long totalSize = param.FileSize.EzLong();
                        byte[] buffer = param.Base64String.EzBase64Byte();
                        if (buffer.EzNotNull())
                        {
                            string folder = RootFolder.EzCreateDirectory();
                            using (FileStream fs = new FileStream(folder.EzCombine(name), FileMode.Append, FileAccess.Write))
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
