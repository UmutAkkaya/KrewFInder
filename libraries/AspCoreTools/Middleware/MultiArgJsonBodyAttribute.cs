using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspCoreTools
{
    /// <summary>
    /// This attribute can be placed on an action (or controller to apply it to all actions) to allow the action
    /// to pull its arguments out of the request body (for POST methods). 
    /// For example an action like <code>Foo(string bar, Person baz, Dictionary&lt;string, int&gt; maps)</code> will
    /// accept a POST with a body like <code>{"bar":"hello", "baz":{"FirstName":"John","LastName":"Smith"}, "maps":{"one":1,"two":17}}</code>
    /// </summary>
    public class MultiArgJsonBodyAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Setting this to true will cause a BadRequest (http 400) response if the parameters in request do not perfectly coincide
        /// with the arguments of the action. That is, if there are any extra parameters in the request, or if any action arguments
        /// are unassigned. If this happens, the response will be returned without the action ever being executed.
        /// </summary>
        public bool Strict { get; set; } = false;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //TODO: Need more checks here like body MIME type, try-catch in case json is invalid, etc
            if (context.HttpContext.Request.Method != HttpMethod.Post.Method)
            {
                return;
            }

            var reqBody = context.HttpContext.Request.Body;
            var reqBodyJson = new StreamReader(reqBody).ReadToEnd();
            var reqJson = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(reqBodyJson) ??
                          new Dictionary<string, JToken>();
            var paramNamesToTypes = context.ActionDescriptor.Parameters.ToDictionary(k => k.Name, k => k.ParameterType);

            //Find JToken and array type args and clear them up. This is a bit of a hack.
            var presetArgs = new List<string>();

            foreach (var param in context.ActionArguments.Keys)
            {
                if (paramNamesToTypes.ContainsKey(param) &&
                    (paramNamesToTypes[param].IsSubclassOf(typeof(JToken)) ||
                     typeof(IEnumerable<JToken>).IsAssignableFrom(paramNamesToTypes[param]) ||
                     paramNamesToTypes[param].IsArray))
                {
                    presetArgs.Add(param);
                }
            }

            presetArgs.ForEach(key => context.ActionArguments.Remove(key));

            foreach (var key in reqJson.Keys)
            {
                if (paramNamesToTypes.ContainsKey(key) &&
                    (!context.ActionArguments.ContainsKey(key) || context.ActionArguments[key] == null))
                {
                    context.ActionArguments[key] = reqJson[key].ToObject(paramNamesToTypes[key]);
                }
                else if (Strict) // We may want to remove this if we're throwing around extra data and it's a pain to strip it before POST.
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        Error = $"{key} in request body does not map to any parameters"
                    });
                    return;
                }
            }

            if (Strict)
            {
                var unassignedArgs = paramNamesToTypes.Keys.Except(context.ActionArguments.Keys).ToArray();
                if (unassignedArgs.Length > 0)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        Error =
                            $"Request did not contain values in root object for {string.Join(", ", unassignedArgs)}."
                    });
                }
            }
        }
    }
}