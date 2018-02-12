using System;
using Ews.RestExtensions;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Mongoose.Common;
using SxL.Common;

namespace CustomSoapProvider
{
    /// <inheritdoc />
    public class CustomUserManager : SoapEwsRestUserManagerBase<CustomProvider, CustomHttpConfiguration, SoapEwsRestUserStore, SoapEwsRestUser, CustomSignInManager, CustomUserManager, CustomOAuthProvider>
    {
        #region Create
        /// <summary>
        /// Creates an instance of this class from the supplied context and options which can then be dependency injected on a "per Owin context" basis.
        /// </summary>
        public static CustomUserManager Create(IdentityFactoryOptions<CustomUserManager> options, IOwinContext context)
        {
            try
            {
                Logger.LogTrace(LogCategory.RestServe, "Creating CustomUserManager", context.Request.Uri);
                return new CustomUserManager();
            }
            catch (Exception ex)
            {
                Logger.LogError(LogCategory.RestServe, ex);
                throw;
            }
        }
        #endregion
    }
}
