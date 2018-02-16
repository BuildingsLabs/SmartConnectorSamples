using Ews.RestExtensions;
using Owin;

namespace CustomSoapProvider
{
    /// <inheritdoc />
    public class CustomHttpConfiguration : SoapEwsRestHttpConfigurationBase<CustomProvider, CustomHttpConfiguration, SoapEwsRestUserStore, SoapEwsRestUser, CustomSignInManager, CustomUserManager, CustomOAuthProvider>
    {
        #region CreateUserManager - Override
        /// <inheritdoc />
        protected override void CreateUserManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<CustomUserManager>(CustomUserManager.Create);
        }
        #endregion
        #region CreateSignInManager - Override
        /// <inheritdoc />
        protected override void CreateSignInManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<CustomSignInManager>(CustomSignInManager.Create);
        }
        #endregion
        #region CreateOAuthProvider - Override
        /// <inheritdoc />
        protected override CustomOAuthProvider CreateOAuthProvider()
        {
            var provider = new CustomOAuthProvider(Name, CacheTenantId, ServerAddress);
            return provider;
        }
        #endregion
    }
}
