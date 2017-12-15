using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Mongoose.Common;
using Mongoose.Common.Api;

namespace CustomRestExtension.Controllers
{
    [Authorize, RoutePrefix("HelloWorld")]
    public class HelloWorldController : RestExtensionControllerBase<MyRestProvider>
    {
        #region Retrieve
        /// <summary>
        /// Returns the greeting 
        /// </summary>
        /// <response code="200">Action successful.</response>

        [HttpGet, Route(""), ResponseType(typeof(string))]
        public HttpResponseMessage GetGreeting()
        {
            Func<HttpResponseMessage> method = delegate
            {
                Func<object, HttpResponseMessage> innerMethod = delegate
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Provider.GetGreeting());
                };
                return this.ExecuteAndLogCall("Retrieve", null, innerMethod);
            };
            return this.ExecuteRequestAndHandleErrors(method);
        }
        #endregion
        #region UpdateId (PUT)
        /// <summary>
        /// Modifies the Id property for the Container with the Id provided to the new value supplied.  Returns the modified Container.
        /// </summary>
        /// <param name="newValue">New Id value.</param>
        /// <response code="200">Action successful.</response>
        [HttpPut, Route("ChangeGreeting"), ResponseType(typeof(string))]
        public HttpResponseMessage UpdateGreeting([FromBody] string newValue)
        {
            Func<HttpResponseMessage> method = delegate
            {
                Func<object, HttpResponseMessage> innerMethod = delegate
                {
                    Provider.UpdateGreeting(newValue);
                    
                    return Request.CreateResponse(HttpStatusCode.OK, newValue);
                };
                return this.ExecuteAndLogCall("UpdateGreeting", new { NewValue = newValue }, innerMethod);
            };
            return this.ExecuteRequestAndHandleErrors(method);
        }
        #endregion
    }
}
