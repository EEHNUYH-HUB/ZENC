using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE;

namespace ZENC.CORE.API.Common.File.Entity
{
    public class DownloadResult : IDisposable
    {
        public Stream DownloadStream { get; set; }
        public string FileName { get; set; }

        public void Dispose()
        {
            if (DownloadStream.EzNotNull())
            {
                DownloadStream.Dispose();
            }
        }
    }
}
