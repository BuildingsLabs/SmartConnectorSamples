using System;
using System.Collections.Generic;
using System.Security.Claims;
using CustomRestExtension.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Mongoose.Common;
using Mongoose.Common.Api;
using SxL.Common;

namespace CustomRestExtension
{
    public class MyRestUserManager : RestUserManagerBase<MyRestProvider, MyRestHttpConfiguration, MyRestUserStore, MyUser, string, MyRestSignInManager, MyRestUserManager, MyRestOAuthProvider>
    {
        #region Constructor
        /// <inheritdoc />
        public MyRestUserManager(IUserStore<MyUser, string> store) : base(store)
        {
        }
        #endregion

        #region VerifyPassword_Subclass - Override
        /// <inheritdoc />
        protected override bool VerifyPassword_Subclass(MyUser user, string password)
        {
            Logger.LogDebug(LogCategory.RestServe, $"MyUserManager is authenticating {user.UserName} with password {password}");
            return user.Password == password;
        }
        #endregion

        #region Create
        /// <summary>
        /// Creates an instance of this class from the supplied context and options which can then be dependency injected on a "per Owin context" basis.
        /// </summary>
        public static MyRestUserManager Create(IdentityFactoryOptions<MyRestUserManager> options, IOwinContext context)
        {
            try
            {
                Logger.LogDebug(LogCategory.RestServe, "Creating MyUserManager", context.Request.Uri);
                return new MyRestUserManager(new MyRestUserStore());
            }
            catch (Exception ex)
            {
                Logger.LogError(LogCategory.RestServe, ex);
                throw;
            }
        }
        #endregion
        #region GenerateClaimsForUser - Override
        /// <inheritdoc />
        protected override List<Claim> GenerateClaimsForUser(MyUser user)
        {
            var claims = base.GenerateClaimsForUser(user);
            

            return claims;
        }
        #endregion
    }
}
