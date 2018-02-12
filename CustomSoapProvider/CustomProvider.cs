using Ews.RestExtensions;

namespace CustomSoapProvider
{
    public class CustomProvider : SoapEwsRestProviderBaseBase<CustomProvider, CustomHttpConfiguration, SoapEwsRestUserStore, SoapEwsRestUser, CustomSignInManager, CustomUserManager, CustomOAuthProvider>
    {
        #region Constructor
        /// <inheritdoc  />
        public CustomProvider()
        {
            HttpConfiguration = new CustomHttpConfiguration(() => Endpoint);
        }
        #endregion

        #region IsLicensed - Override
        /// <inheritdoc />
        public override bool IsLicensed => false;
        #endregion
    }
}
