using System.Threading.Tasks;
using CustomRestExtensionStub.Model;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using Mongoose.Common.Api;

namespace CustomRestExtensionStub
{
    public class MyRestOAuthProvider : RestOAuthProviderBase<MyRestProvider, MyRestHttpConfiguration, MyRestUserStore, MyUser, string, MyRestSignInManager, MyRestUserManager, MyRestOAuthProvider>
    {
        #region Constructor
        /// <summary>
        /// Creates an instance of this class with default behavior
        /// </summary>
        /// <param name="publicClientId">Value for PublicClientId property.</param>
        public MyRestOAuthProvider(string publicClientId)
            : base(publicClientId)
        {
            
        }
        #endregion

        #region GetUserManagerFromContextAndHydrateProperties - Override
        /// <inheritdoc />
        protected override MyRestUserManager GetUserManagerFromContextAndHydrateProperties(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<MyRestUserManager>();


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
