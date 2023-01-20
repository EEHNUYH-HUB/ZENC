using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE.API.Common.Auth;
using ZENC.CORE;

namespace ZENC.CORE.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthFilterAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                         .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

            if (hasAllowAnonymous)
                return;

            var key = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (key.ExIsNull() || AuthFactory.SingInstance.ExIsNull() || !AuthFactory.SingInstance.IsValidate(key))
            {
                var payload = new APIErrorDetails()
                {
                    StatusCode = (int)StatusCodes.Status401Unauthorized,
                    Message = "Unauthorized.",
                    Source = "Authentication"
                };

                context.Result = new UnauthorizedObjectResult(payload);
            }
        }
    }
}
