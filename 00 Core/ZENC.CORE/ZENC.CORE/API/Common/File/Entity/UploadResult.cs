using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZENC.CORE.API.Common.File.Entity
{
    public enum UploadResultType
    {
        PROGRESS,COMPLETED,ERROR
    }
    public class UploadResult
    {
        public UploadResultType ResultType { get; set; }
        public string ErrorMsg { get; set; }

    }
}
