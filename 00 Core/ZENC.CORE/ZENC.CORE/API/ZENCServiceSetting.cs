using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE.API.Common.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog;
using ZENC.CORE.API.Common.File;
using ZENC.CORE.API.Common;
using System.Text.Json;
using Newtonsoft.Json;
namespace ZENC.CORE.API
{
    public class ZENCServiceSetting
    {
        IServiceCollection services;
        public ZENCServiceSetting(IServiceCollection services)
        {
            this.services = services;
        }

        public void Init()
        {
            #region ASP.NET Core 압축 설정
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml", "application/json" });

            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            #endregion

            services.AddMemoryCache();
            services.AddControllers().AddNewtonsoftJson();
            
            //services.AddControllers().addnew
            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            //    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //});
        }

        public void InitAuthenticator(IAuthenticator auth)
        {
            AuthFactory.SingInstance = auth;
            this.services.AddSingleton<IAuthenticator>((svc) =>
            {
                return auth;
            });
        }
        public void InitSmartSql(string configPath)
        {
            this.services.AddSingleton<SmartSqlMapper>();

            this.services.AddSmartSql((builder) =>
            {
                builder.UseXmlConfig(SmartSql.ConfigBuilder.ResourceType.File, configPath);
            });
        }

        public void InitFileHandler(IFileHandler handler)
        {
            this.services.AddSingleton<IFileHandler>((svc) =>
            {
                return handler;
            });
        }
        public void InitNLogConfig(string configPath)
        {
            LogManager.LoadConfiguration(configPath);
        }

        public string AddCors(params string[] urls)
        {
            var allowOrigins = "allowOrigins";
            this.services.AddCors(options =>
            {
                options.AddPolicy(name: allowOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins(urls).AllowAnyMethod()
                                                .AllowAnyHeader()
                                                .WithExposedHeaders("content-disposition")
                                                .WithMethods("GET", "POST").AllowCredentials();
                                  });
            });

            
            return allowOrigins;
        }

    }
}
