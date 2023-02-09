using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE.API.Common.Auth;

namespace ZENC.AZURE.Auth
{
    public class OAuth2AzureAD : IAuthenticator
    {
        public string Key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int Expired { get; set; }

       

        public string GenerateKey(Dictionary<string, object> param)
        {
            return GetToken().Result;
        }



        private static async Task<string> GetToken()
        {

            return null;
            //string clientID = "38f39ceb-5d5d-4424-be84-8972dbc771fe\r\n";

            //string AuthEndPoint = "https://login.microsoftonline.com/{0}/oauth2/token";
            //string tenant_id = "785087ba-1e72-4e7d-b1d1-4a9639137a66";


            //var app = PublicClientApplicationBuilder.Create(clientID).WithAuthority(AzureCloudInstance.AzurePublic, tenant_id).WithRedirectUri("http://localhost").Build();

            //List<string> scopes = new List<string> { "user.read" };

            //var result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            //return result.AccessToken;


            //string clientId = "74a7177a-327b-43de-b8e9-713df68777da";
            //string tenantId = "785087ba-1e72-4e7d-b1d1-4a9639137a66";
            //string[] scopes = { "user.read" };

            //var app = PublicClientApplicationBuilder.Create(clientId)
            //        .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
            //        .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
            //        .Build();

            //app.UserTokenCache

            //return result.AccessToken;
        }

        // string GetAccessToken(string clientId, string clientkey)
        //{
        //    string AuthEndPoint = "https://login.microsoftonline.com/{0}/oauth2/token";
        //    string TenantId = "785087ba-1e72-4e7d-b1d1-4a9639137a66";
        //    string authority = string.Format(CultureInfo.InvariantCulture, AuthEndPoint, TenantId);
        //    AuthenticationContext authContext = new AuthenticationContext(authority);
        //    ClientCredential clientCredential = new ClientCredential(clientId, clientkey);
        //    string resourceUri = "https://analysis.windows.net/powerbi/api";
        //    AuthenticationResult authenticationResult = authContext.AcquireTokenAsync(resourceUri, clientCredential).Result;


        //    return authenticationResult.AccessToken;
        //}


       

        public Dictionary<string, string> GetInfo(string token)
        {
            return null;
        }

        public bool IsValidate(string token)
        {
            return false;
        }
    }

}
