using System.Threading.Tasks;
using CustomRestExtension.Model;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using Mongoose.Common.Api;

namespace CustomRestExtension
{
    public class MyRestOAuthProvider : RestOAuthProviderBase<MyRestProvider, MyRestHttpConfiguration, MyRestUserStore, MyUser, string, MyRestSignInManager, MyRestUserManager, MyRestOAuthProvider>
    {
        #region Constructor
        /// <summary>
        /// Creates an instance of this class with default behavior
        /// </summary>
        /// <param name="publicClientId">Value for PublicClientId property.</param>
        /// <para name="configuredUser">Credentials allowed to use the REST endpoint.</para>
        public MyRestOAuthProvider(string publicClientId, string cacheTenantId, MyUser configuredUser)
            : base(publicClientId, cacheTenantId)
        {
            ConfiguredUser = configuredUser;
        }
        #endregion
        #region ConfiguredUser
        protected MyUser ConfiguredUser { get; }
        #endregion

        #region GetUserManagerFromContextAndHydrateProperties - Override
        /// <inheritdoc />
        protected override MyRestUserManager GetUserManagerFromContextAndHydrateProperties(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<MyRestUserManager>();
            // Add any things to the UserManager which we're aware of because we were spun up by the service under the auspices of MyRestHttpConfiguration
            userManager.ConfiguredUser = ConfiguredUser;
            return userManager;
        }
        #endregion
        #region GetUserRequestingAuthentication - Override
        /// <inheritdoc />
        protected override Task<MyUser> GetUserRequestingAuthentication(MyRestUserManager userManager, OAuthGrantResourceOwnerCredentialsContext context)
        {
            return userManager.FindAsync(context.UserName, context.Password);
        }
        #endregion
    }
}
