using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Mongoose.Common.Attributes;
using SxL.Common;

namespace CustomRestExtension.Model
{
    /// <summary>
    /// User class for authenticating client requests
    /// </summary>
    public class MyUser : IUser<string>, ITraversable
    {
        #region Id (IUser Member)
        /// <inheritdoc />
        public string Id => UserName;
        #endregion
        #region UserName (IUser Member)
        /// <inheritdoc />
        [Required]
        public string UserName { get; set; }
        #endregion
        #region Password
        [Required, EncryptedString]
        public string Password { get; set; }
        #endregion
    }
}
