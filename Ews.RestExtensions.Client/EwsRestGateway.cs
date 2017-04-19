using System;
using System.Linq;
using System.Security;
using Microsoft.Rest;

namespace Ews.RestExtensions.Client.Proxy
{
    public partial class EwsRestGateway
    {
        private BearerToken _token;
        private readonly SecureString _userName = new SecureString();
        private readonly SecureString _password = new SecureString();

        #region HasValidCredentials
        /// <summary>
        /// If true, the client was intantiated with valid credentials and has a valid Bearer token.
        /// </summary>
        public bool HasValidCredentials => _token != null;
        #endregion
        #region HasTokenExpired
        /// <summary>
        /// Returns true if no token exists or it has expired
        /// </summary>
        /// <returns></returns>
        public bool HasTokenExpired()
        {
            return _token?.HasExpired ?? true;
        }
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
            var client = new EwsRestGateway
            {
                _token = BearerToken.ObtainToken(baseUri, userName, password, tokenEndpoint)

            };
            client.BaseUri = baseUri;
            client._password.LoadValue(password);
            client._userName.LoadValue(userName);

            InitializeServiceClient(client);
            return client;
        }
        #endregion

        #region ReAuthenticate
        /// <summary>
        /// Obtains a new Bearer Token. Can only be called after a valid Connect call
        /// </summary>
        public void ReAuthenticate()
        {
            if (_token == null) throw new InvalidOperationException("Cannot re-authenticate when authentication has never succeeded.");
            _token = BearerToken.ObtainToken(_token.TokenUri, _userName.ExtractValue(), _password.ExtractValue(), string.Empty);
            InitializeServiceClient(this);
            if (_token != null) Credentials = new TokenCredentials(_token?.access_token, _token?.token_type);
            Credentials?.InitializeServiceClient(this);
        } 
        #endregion

        #region InitializeServiceClient
        private static void InitializeServiceClient(EwsRestGateway client)
        {
            if (client._token != null)
            {
                client.Credentials = new TokenCredentials(client._token?.access_token, client._token?.token_type);
                client.Credentials.InitializeServiceClient(client);

            }
            else
            {
                client.Credentials = null;
            }
        } 
        #endregion
    }
}
