using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Http.Dispatcher;
using CustomRestExtensionStub.Model;
using Mongoose.Common.Api;
using Mongoose.Common.Attributes;
using Owin;

namespace CustomRestExtensionStub
{
    public class MyRestHttpConfiguration : RestHttpConfigurationBase<MyRestProvider, MyRestHttpConfiguration, MyRestUserStore, MyUser, string, MyRestSignInManager, MyRestUserManager, MyRestOAuthProvider>
    {
        #region Constructor
        /// <inheritdoc />
        public MyRestHttpConfiguration(Func<string> endpointUrl) : base(endpointUrl)
        {
        }
        #endregion

        #region AssembliesResolver - Override
        /// <summary>
        /// Configures the CustomAssemblyResolver for this assembly.
        /// </summary>
        protected override IAssembliesResolver AssembliesResolver => new CustomAssemblyResolver(new List<Assembly> { Assembly.GetAssembly(typeof(MyRestProvider)) });
        #endregion

        // TODO - Any properties you need to have configured would go here.  Use standard validation attributes as appropriate.
        //        If those are required to connect to the backend (as they propbably are) then they need to be passed on the chain.

        #region SomeInteger
        /// <summary>
        /// Some integer value that's important.
        /// </summary>
        [Required, Range(1, int.MaxValue), Tooltip("Some helpful info we can provide to the configuring person")]
        public int SomeInteger { get; set; }
        #endregion

        #region CreateUserManager - Override
        /// <inheritdoc />
        protected override void CreateUserManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<MyRestUserManager>(MyRestUserManager.Create);
        }
        #endregion
        #region CreateSignInManager - Override
        /// <inheritdoc />
        protected override void CreateSignInManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<MyRestSignInManager>(MyRestSignInManager.Create);
        }
        #endregion
        #region CreateOAuthProvider - Override
        /// <inheritdoc />
        protected override MyRestOAuthProvider CreateOAuthProvider()
        {
            var provider = new MyRestOAuthProvider(Name);
            return provider;
        }
        #endregion
    }
}