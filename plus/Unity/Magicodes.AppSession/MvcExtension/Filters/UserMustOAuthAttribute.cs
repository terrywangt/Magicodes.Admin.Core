using System;
using Abp.Dependency;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Magicodes.AppSession.MvcExtension.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class UserIgnoreOAuthAttribute : AuthorizeAttribute, IAuthorizationFilter, ITransientDependency
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
        }
    }
}