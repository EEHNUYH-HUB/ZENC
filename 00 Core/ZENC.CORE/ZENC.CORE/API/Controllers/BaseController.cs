using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE.API.Common.Auth;
using ZENC.CORE.API.Filters;

namespace ZENC.CORE.API.Controllers
{
    [ApiController]
    [AuthFilterAttribute]
    [Route("api/[controller]/[action]")]
    public class BaseController : ControllerBase
    {
        public IAuthenticator CurrentContext { get { return AuthFactory.SingInstance; } }
        public BaseController(IAuthenticator ctx)
        {
            AuthFactory.SingInstance = ctx;
        }
    }
}
