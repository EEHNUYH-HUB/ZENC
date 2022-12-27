
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using static System.Net.WebRequestMethods;
using System.Threading;
using System.Net.Http;

namespace ZENC.CORE.API.Client
{
    public class RestAPIHandler
    {
        private readonly TimeSpan timeout;
        private readonly DefaultContractResolver resolver;
        private const string CLIENTUSERAGENT = "my-api-client-v1";
        private const string MEDIATYPEJSON = "application/json";
        public string BaseUrl { get; set; }
        public Action<HttpClient> OnHeaderSettingDelegate { get; set; }


        public RestAPIHandler(string baseUrl, Action<HttpClient> onHeaderDelegate,
            DefaultContractResolver resolver = null
            ,
            TimeSpan?timeout = null)
        {
            this.OnHeaderSettingDelegate = onHeaderDelegate;
            this.resolver = resolver ?? new DefaultContractResolver();
            //new CamelCasePropertyNamesContractResolver()
            this.timeout = timeout ?? TimeSpan.FromSeconds(90);
            this.BaseUrl = baseUrl;
        }
        private HttpClient GetHttpClient()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            HttpClient httpClient = new HttpClient(httpClientHandler, false)
            {
                Timeout = timeout
            };

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(CLIENTUSERAGENT);

            if (!string.IsNullOrWhiteSpace(BaseUrl))
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
            }

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIATYPEJSON));

            if (OnHeaderSettingDelegate != null)
            {
                OnHeaderSettingDelegate(httpClient);

            }

            return httpClient;
        }

        public string Put(string url, HttpContent content)
        {
            using (HttpClient httpClient = GetHttpClient())
            {
                using (var response = httpClient.PutAsync(url, content).Result)
                {

                    var v = response.Content.ReadAsStringAsync().Result;

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    else if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return v;
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    return v;
                }
            }
        }

        public string Get(string url)
        {
            using (HttpClient httpClient = GetHttpClient())
            {

                using (var response = httpClient.GetAsync(url).Result)
                {

                    var v = response.Content.ReadAsStringAsync().Result;

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {

                        response.EnsureSuccessStatusCode();

                    }
                    else if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return v;
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    return v;
                }
            }
        }

        public string Delete(string url)
        {
            using (HttpClient httpClient = GetHttpClient())
            {
                using (var response = httpClient.DeleteAsync(url).Result)
                {
                    var v = response.Content.ReadAsStringAsync().Result;

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {

                        response.EnsureSuccessStatusCode();

                    }
                    else if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return v;
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    return v;
                }
            }
        }

        public string Post(string url, object input)
        {
            using (HttpClient httpClient = GetHttpClient())
            {
                using (var requestContent = new StringContent(ConvertToJsonString(input), Encoding.UTF8, MEDIATYPEJSON))
                {
                    using (var response = httpClient.PostAsync(url, requestContent).Result)
                    {

                        var v = response.Content.ReadAsStringAsync().Result;

                        if (response.StatusCode == HttpStatusCode.Unauthorized || v.ToString().Contains("401(Unauthorized)") || v.ToString().Contains("(Unauthorized)"))
                        {

                            response.EnsureSuccessStatusCode();

                        }
                        else if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return v;
                        }
                        else
                        {
                            response.EnsureSuccessStatusCode();
                        }
                        return v;
                    }
                }
            }

        }
        public string Patch(string url, object input)
        {
            using (HttpClient httpClient = GetHttpClient())
            {


                //using (var requestContent = new StringContent(ConvertToJsonString(input), Encoding.UTF8, MEDIATYPEJSON))
                {

                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);

                    request.Content = new StringContent(ConvertToJsonString(input), System.Text.Encoding.UTF8, MEDIATYPEJSON);


                    //httpClient.SendAsync()
                    using (var response = httpClient.SendAsync(request).Result)
                    {


                        var v = response.Content.ReadAsStringAsync().Result;

                        if (response.StatusCode == HttpStatusCode.Unauthorized || v.ToString().Contains("401(Unauthorized)") || v.ToString().Contains("(Unauthorized)"))
                        {

                            response.EnsureSuccessStatusCode();

                        }
                        else if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return v;
                        }
                        else
                        {
                            response.EnsureSuccessStatusCode();
                        }
                        return v;
                    }
                }
            }

        }
        //public string Patch(string url, object input)
        //{
        //    using (HttpClient httpClient = GetHttpClient())
        //    {
        //        using (var requestContent = new StringContent(ConvertToJsonString(input), Encoding.UTF8, MEDIATYPEJSON))
        //        {
        //            using (var response = httpClient.PatchAsync(url, requestContent).Result)
        //            {

        //                var v = response.Content.ReadAsStringAsync().Result;

        //                if (response.StatusCode == HttpStatusCode.Unauthorized || v.EzToString().Contains("401(Unauthorized)") || v.EzToString().Contains("(Unauthorized)"))
        //                {

        //                    response.EnsureSuccessStatusCode();

        //                }
        //                else if (response.StatusCode == HttpStatusCode.OK)
        //                {
        //                    return v;
        //                }
        //                else
        //                {
        //                    response.EnsureSuccessStatusCode();
        //                }
        //                return v;
        //            }
        //        }
        //    }

        //}

        public T Post<T>(string url, object input)
        {
            string str = Post(url, input);
            return JsonConvert.DeserializeObject<T>(str);
            
        }

        public T Get<T>(string url)
        {
            string str = Get(url);
            return JsonConvert.DeserializeObject<T>(str);

        }
        private string ConvertToJsonString(object obj )
        {
         
            if (obj == null)
            {
                return string.Empty;
            }
            
            var rtn = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = resolver
            });

            return rtn;
        }

        private static string NormalizeBaseUrl(string url)
        {
            return url.EndsWith("/") ? url : url + "/";
        }

    }
}
