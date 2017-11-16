using System.ComponentModel.DataAnnotations;
using CustomRestExtensionStub.Model;
using Mongoose.Common;
using Mongoose.Common.Api;
using Mongoose.Common.Attributes;

namespace CustomRestExtensionStub
{
    /// <summary>
    /// Class which is availble to all controllers.  This class should provide all data to be served.
    /// </summary>
    [ConfigurationDefaults("Sample REST Provider Class", "Sample strucuture for REST Extension Development")]
    public class MyRestProvider : RestProviderBase<MyRestProvider, MyRestHttpConfiguration, MyRestUserStore, MyUser, string, MyRestSignInManager, MyRestUserManager, MyRestOAuthProvider>, IProviderWithCache
    {
        #region Constructor
        /// <inheritdoc />
        public MyRestProvider()
        {
            HttpConfiguration = new MyRestHttpConfiguration(() => Endpoint);
        }
        #endregion

        #region CacheTenantId (IProviderWithCache Member)
        /// <inheritdoc />
        public string CacheTenantId => HttpConfiguration?.CacheTenantId;
        #endregion
        #region InMemoryCache (IProviderWithCache Member)
        /// <inheritdoc />
        public ICache InMemoryCache => MongooseObjectFactory.Current.GetInstance<ICache>();
        #endregion

        #region HttpConfiguration
        [Required]
        public MyRestHttpConfiguration HttpConfiguration { get; set; }
        #endregion
        #region GetHttpConfiguration - Override
        /// <inheritdoc />
        public override CoreHttpConfigurationBase GetHttpConfiguration()
        {
            return HttpConfiguration;
        }
        #endregion
        #region IsLicensed - Override
        /// <inheritdoc />
        public override bool IsLicensed => false;
        #endregion

        #region Dispose - Override
        private bool _disposed;
        /// <inheritdoc />
        public override void Dispose()
        {
            if (_disposed) return;

            // Dispose anything that you need to

            _disposed = true;
        }
        #endregion
    }
}
