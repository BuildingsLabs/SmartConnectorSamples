using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Mongoose.Common;
using Mongoose.Common.Api;

namespace CustomRestExtension.Controllers
{
    [Authorize, RoutePrefix("Hello")]
    public class HelloWorldController : RestExtensionControllerBase<MyRestProvider>
    {
        #region Retrieve
        [HttpGet, Route(""), ResponseType(typeof(string))]
        public HttpResponseMessage HellowWorld()
        {
            Func<HttpResponseMessage> method = delegate
            {
                Func<object, HttpResponseMessage> innerMethod = delegate
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Hello World");
                };
                return this.ExecuteAndLogCall("Retrieve", null, innerMethod);
            };
            return this.ExecuteRequestAndHandleErrors(method);
        }
        #endregion
    }
}
