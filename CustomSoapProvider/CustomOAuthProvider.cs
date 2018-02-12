using Ews.RestExtensions;

namespace CustomSoapProvider
{
    /// <inheritdoc />
    public class CustomOAuthProvider : SoapEwsRestOAuthProviderBase<CustomProvider, CustomHttpConfiguration, SoapEwsRestUserStore, SoapEwsRestUser, CustomSignInManager, CustomUserManager, CustomOAuthProvider>
    {
        #region Constructor
        /// <inheritdoc />
        public CustomOAuthProvider(string publicClientId, string serverAddress, string cacheTenantId) : base(publicClientId, serverAddress, cacheTenantId)
        {
        }
        #endregion
    }
}
