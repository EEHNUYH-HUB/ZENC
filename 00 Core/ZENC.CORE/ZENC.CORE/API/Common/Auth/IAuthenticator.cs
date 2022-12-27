using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE.API.Parameters;

namespace ZENC.CORE.API.Common.Auth
{
    public interface IAuthenticator
    {
        static IAuthenticator SingInstance { get; internal set; }


        string Key { get; }
        string Issuer { get; }
        string Audience { get; }
        int Expired { get; }
        string GenerateKey(Dictionary<string, object> param);
        bool IsValidate(string token);

        Dictionary<string, string> GetInfo(string token);

    }
}
