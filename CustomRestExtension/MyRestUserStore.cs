using System.Threading.Tasks;
using CustomRestExtension.Model;
using Mongoose.Common.Api;

namespace CustomRestExtension
{
    /// <summary>
    /// This class works in cooperation with the MyRestUserManager to do authentication.
    /// 
    /// If your users are stored in a database, then return then from here.  If configurable properties are required on your Store, then they need to be made part of the 
    /// HttpConfiguration and set here via OAuthProvider.
    /// </summary>
    public class MyRestUserStore : RestUserStoreBase<MyUser, string>
    {
        #region ConfiguredUser
        /// <summary>
        /// User configured for this endpoint.  Typically
        /// </summary>
        public MyUser ConfiguredUser
        {
            get;
            set;
        }
        #endregion

        #region FindByIdAsync - Override
        /// <inheritdoc />
        public override Task<MyUser> FindByIdAsync(string userId)
        {
            // Typically, ConfiguredUser would actually be data we would use to retrieve the user from our backing store.  But that's not how this is example is written.
            return Task.FromResult(ConfiguredUser);

        }
        #endregion
        #region FindByNameAsync - Override
        /// <inheritdoc />
        public override Task<MyUser> FindByNameAsync(string userName)
        {
            // Typically, ConfiguredUser would actually be data we would use to retrieve the user from our backing store.  But that's not how this is example is written.
            return Task.FromResult(ConfiguredUser);
        }
        #endregion
    }
}
