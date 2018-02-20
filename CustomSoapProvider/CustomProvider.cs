using Ews.RestExtensions;
using Ews.RestExtensions.Models;
using Mongoose.Common;
using Mongoose.Common.Api;
using SxL.Common;

namespace CustomSoapProvider
{
    public class CustomProvider : SoapEwsRestProviderBase<CustomProvider, CustomHttpConfiguration, SoapEwsRestUserStore, SoapEwsRestUser, CustomSignInManager, CustomUserManager, CustomOAuthProvider>
    {
        #region IsLicensed - Override
        /// <inheritdoc />
        public override bool IsLicensed => false;
        #endregion

        public override ContainerModel RetrieveRootContainer(BaseApiController controller)
        {
            Logger.LogDebug(LogCategory.RestServe,Name, CacheTenantId);
            var root = base.RetrieveRootContainer(controller);

            root.Description = "Hijacked description";

            return root;
        }
    }
}
