using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE.API.Common.File.Entity;
using ZENC.CORE.API.Parameters;

namespace ZENC.CORE.API.Common.File
{
    public interface IFileHandler
    {
        long BufferSize { get; }
        DownloadResult Read(string staticID);
        //DownloadResult Get(string param);

        UploadResult Write(FileParameter param);
    }
}
