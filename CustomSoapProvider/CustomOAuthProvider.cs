using Ews.RestExtensions;

namespace CustomSoapProvider
{
    /// <inheritdoc />
    public class CustomOAuthProvider : SoapEwsRestOAuthProviderBase<CustomProvider, CustomHttpConfiguration, SoapEwsRestUserStore, SoapEwsRestUser, CustomSignInManager, CustomUserManager, CustomOAuthProvider>
    {
        #region Constructor
        /// <inheritdoc />
        public CustomOAuthProvider(string publicClientId, string cacheTenantId, string serverAddress) : base(publicClientId, cacheTenantId, serverAddress)
        {
        }
        #endregion
    }
}
