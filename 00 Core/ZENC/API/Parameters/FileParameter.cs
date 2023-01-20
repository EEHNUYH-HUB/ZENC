﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZENC.CORE.API.Parameters
{
    public class FileParameter
    {
        public string StaticID { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string Base64String { get; set; }
        public Dictionary<string,object> Parameters { get; set; }
    }
}
