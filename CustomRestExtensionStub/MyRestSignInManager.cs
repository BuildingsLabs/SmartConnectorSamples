using CustomRestExtensionStub.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Mongoose.Common;
using Mongoose.Common.Api;
using SxL.Common;

namespace CustomRestExtensionStub
{
    public class MyRestSignInManager : RestSignInManagerBase<MyUser, string>
    {
        #region Constructor
        /// <inheritdoc />
        public MyRestSignInManager(UserManager<MyUser, string> userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
        #endregion

        #region Create - Static
        /// <summary>
        /// Creates an instance of this class from the supplied context and options which can then be dependency injected on a "per Owin context" basis.
        /// </summary>
        public static MyRestSignInManager Create(IdentityFactoryOptions<MyRestSignInManager> options, IOwinContext context)
        {
            Logger.LogDebug(LogCategory.RestServe, "Creating MySignInManager", context.Request.Uri);
            return new MyRestSignInManager(context.GetUserManager<MyRestUserManager>(), context.Authentication);
        }
        #endregion
    }
}
