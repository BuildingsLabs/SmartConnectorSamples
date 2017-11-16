using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Http.Dispatcher;
using CustomRestExtension.Model;
using Mongoose.Common.Api;
using Mongoose.Common.Attributes;
using Owin;

namespace CustomRestExtension
{
    public class MyRestHttpConfiguration : RestHttpConfigurationBase<MyRestProvider, MyRestHttpConfiguration, MyRestUserStore, MyUser, string, MyRestSignInManager, MyRestUserManager, MyRestOAuthProvider>
    {
        #region Constructor
        /// <inheritdoc />
        public MyRestHttpConfiguration(Func<string> endpointUrl) : base(endpointUrl)
        {
            ClientCredentials = new MyUser();
        }
        #endregion

        #region AssembliesResolver - Override
        /// <summary>
        /// Configures the CustomAssemblyResolver for this assembly.
        /// </summary>
        protected override IAssembliesResolver AssembliesResolver => new CustomAssemblyResolver(new List<Assembly> { Assembly.GetAssembly(typeof(MyRestProvider)) });
        #endregion

        #region Administrator
        /// <summary>
        /// Using a configurable approach like this is technically OK, but not best practice.  Typical systems would already have a way to authenticate users an not do this.  
        /// It does show how configured data can be pushed around the Owin middleware to where it is later needed at run time.
        /// </summary>
        [Required, Tooltip("Credentials to be used when clients authenticate")]
        public MyUser ClientCredentials { get; set; }
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
            var provider = new MyRestOAuthProvider(Name, ClientCredentials);
            return provider;
        }
        #endregion
    }
}