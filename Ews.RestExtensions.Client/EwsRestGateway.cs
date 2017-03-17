using System;
using System.Web;
using Microsoft.Rest;

namespace Ews.RestExtensions.Client.Proxy
{
    public partial class EwsRestGateway
    {
        #region HasValidCredentials
        /// <summary>
        /// If true, the client was intantiated with valid credentials and has a valid Bearer token.
        /// </summary>
        public bool HasValidCredentials { get; private set; }
        #endregion
        
        #region Connect
        /// <summary>
        /// Creates a connection to an EwsRestGateway client.
        /// </summary>
        /// <param name="baseUri">URI where the client is located.</param>
        /// <param name="userName">UserName to authenticate with.</param>
        /// <param name="password">Password to authenticate with.</param>
        /// <param name="tokenEndpoint">Path where the bearer token can be retrieved.</param>
        public static EwsRestGateway Connect(string baseUri, string userName, string password, string tokenEndpoint = "GetToken")
        {
            return Connect(new Uri(baseUri), userName, password, tokenEndpoint);
        }
        /// <summary>
        /// Creates a connection to an EwsRestGateway client.
        /// </summary>
        /// <param name="baseUri">URI where the client is located.</param>
        /// <param name="userName">UserName to authenticate with.</param>
        /// <param name="password">Password to authenticate with.</param>
        /// <param name="tokenEndpoint">Path where the bearer token can be retrieved.</param>
        public static EwsRestGateway Connect(Uri baseUri, string userName, string password, string tokenEndpoint = "GetToken")
        {
            var token = BearerToken.ObtainToken(baseUri, userName, password, tokenEndpoint);
            var client = new EwsRestGateway
            {
                HasValidCredentials = token != null
            };

            if (token != null) client.Credentials = new TokenCredentials(token?.access_token, token?.token_type);
            client.Credentials?.InitializeServiceClient(client);
            return client;
        }
        #endregion
    }
}
