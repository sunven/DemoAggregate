using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApiDemo.Common;

namespace WebApiDemo.Filters
{
    public class ResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            if (actionExecutedContext.Exception != null)
            {
                return;
            }

            var isActionResult =
                typeof(IHttpActionResult).IsAssignableFrom(actionExecutedContext.ActionContext.ActionDescriptor
                    .ReturnType);
            if (isActionResult)
            {
                return;
            }

            var httpStatusCodes = new List<HttpStatusCode>
            {
                HttpStatusCode.OK,
                HttpStatusCode.NoContent
            };

            if (!httpStatusCodes.Contains(actionExecutedContext.Response.StatusCode))
            {
                throw new Exception();
            }

            var httpResponseMessage = actionExecutedContext.Response.Content as ObjectContent;
            if (httpResponseMessage?.Value is ResultMessage)
            {
                return;
            }

            var apiMessage = new ResultMessage(
                ResultState.Success,
                "success",
                httpResponseMessage?.Value
            );

            var jsonMediaTypeFormatter =
                new JsonMediaTypeFormatter
                {
                    SerializerSettings =
                    {
                        Formatting = Formatting.Indented,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                };
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                HttpStatusCode.OK,
                apiMessage,
                jsonMediaTypeFormatter);
        }
    }
}