using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE.API.Common.Auth;
using ZENC.CORE.API.Parameters;
using ZENC.CORE.Util;

namespace ZENC.CORE.API.Controllers
{
    public class DynamicController : BaseController
    {
        public DynamicController(IAuthenticator ctx) : base(ctx) { }
        [HttpPost]
        public object LoadAssembly([FromBody] DynamicParameter info)
        {
            return AssemblyLoader.Run(info);
        }
    }
}
