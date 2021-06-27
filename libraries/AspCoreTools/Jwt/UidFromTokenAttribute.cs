using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspCoreTools.Jwt
{
    public class UidFromTokenAttribute : Attribute, IBinderTypeProviderMetadata
    {
        public BindingSource BindingSource => BindingSource.Custom;
        public Type BinderType => typeof(TokenBinder);
    }

    public class TokenBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var httpCtx = bindingContext.HttpContext;
            
            //TODO: Support variable keys
            var key = RoleType.UserIdClaimType;

            var value = httpCtx.User.FindFirst(key)?.Value;

            bindingContext.Model = value;
            bindingContext.Result = ModelBindingResult.Success(value);

            return Task.CompletedTask;
        }
    }
}
