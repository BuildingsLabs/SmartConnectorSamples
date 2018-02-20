using Ews.RestExtensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace CustomSoapProvider
{
    public class CustomSignInManager : SoapEwsRestSignInManagerBase<SoapEwsRestUser>
    {
        #region Constructor
        /// <inheritdoc />
        public CustomSignInManager(UserManager<SoapEwsRestUser, string> userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
        #endregion

        #region Create
        /// <summary>
        /// Creates an instance of this class from the supplied context and options which can then be dependency injected on a "per Owin context" basis.
        /// </summary>
        public static CustomSignInManager Create(IdentityFactoryOptions<CustomSignInManager> options, IOwinContext context)
        {
            return new CustomSignInManager(context.GetUserManager<CustomUserManager>(), context.Authentication);
        }
        #endregion
    }
}
