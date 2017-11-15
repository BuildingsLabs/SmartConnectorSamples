using System.ComponentModel.DataAnnotations;
using CustomRestExtensionStub.Model;
using Mongoose.Common.Api;

namespace CustomRestExtensionStub
{
    public class MyRestProvider : RestProviderBase<MyRestProvider, MyRestHttpConfiguration, MyRestUserStore, MyUser, string, MyRestSignInManager, MyRestUserManager, MyRestOAuthProvider>
    {
        #region Constructor
        /// <inheritdoc />
        public MyRestProvider()
        {
            HttpConfiguration = new MyRestHttpConfiguration(() => Endpoint);
        }
        #endregion

        #region HttpConfiguration
        /// <summary>
        /// HttpConfiguration for this provider.
        /// </summary>
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
