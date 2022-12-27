using System;
using System.Collections.Generic;
using System.Text;
using ZENC.CORE.API.Common.Auth;

namespace ZENC.CORE.API.Common.Auth
{
    public class AuthFactory
    {
        public static IAuthenticator SingInstance { get; set; }
    }
}
