using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApiDemo.Common;

namespace WebApiDemo.Filters
{
    public class ApiExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var jsonMediaTypeFormatter =
                new JsonMediaTypeFormatter
                {
                    SerializerSettings =
                    {
                        Formatting = Formatting.Indented,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                };
            var result = context.Request.CreateResponse(HttpStatusCode.OK,
                                new ResultMessage(ResultState.Fail, context.Exception.Message, null), jsonMediaTypeFormatter);
            context.Response = result;
        }
    }
}