using System;
using System.Collections.Generic;
using Ews.Server.Contract;
using Mongoose.Common;
using Mongoose.Common.Data.Licensing;
using Mongoose.Ews.Server.Data;

namespace SmartConnector.WeatherExtension.EwsServer
{
    /// <summary>
    /// EwsServiceHost subclass to inject our custom endpoint and controller functionality into SmartConnector.
    /// </summary>
    public class CustomEwsServiceHost : EwsServiceHost
    {
        #region Constructor
        public CustomEwsServiceHost(Mongoose.Ews.Server.Data.EwsServer serverConfiguration)
            : base(typeof(CustomDataExchange), serverConfiguration) { }
        #endregion

        #region IsLicensed - Override (ILicensable Member)
        /// <summary>
        /// Indicates to the runtime framework whether the author has not requested Extension License enforcement.
        /// </summary>
        public override bool IsLicensed => false;
        #endregion
        #region ValidateCustomLicenseFeatures - Override (ILicensable Member)
        /// <summary>
        /// Custom feature license enforcement is up to the implementing extension.
        /// </summary>
        public override IEnumerable<Prompt> ValidateCustomLicenseFeatures(ExtensionLicense license)
        {
            throw new NotImplementedException("Implementing class does not support licensing");
        }
        #endregion

        #region ProvisionEndpoint - Override
        protected override void ProvisionEndpoint()
        {
            AddServiceEndpoint(typeof(IDataExchange), CreateBinding(IsHttps), ServerAddress);
        }
        #endregion
    }
}
