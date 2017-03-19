using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Web;

namespace Ews.RestExtensions.Client
{
    public static class ExtensionMethods
    {
        #region UrlEncodeToString (string)
        /// <summary>
        /// Returns a string which has been URL encoded
        /// </summary>
        public static string UrlEncodeToString(this string stringToEncode)
        {
            return string.IsNullOrEmpty(stringToEncode) ? null : HttpUtility.UrlEncode(stringToEncode);
        }
        #endregion

        #region LoadValue (SecureString)
        /// <summary>
        /// Safely secures insecureString into secureString
        /// </summary>
        public static void LoadValue(this SecureString secureString, string insecureString)
        {
            secureString.Clear();
            insecureString.ToCharArray().ToList().ForEach(secureString.AppendChar);
        }
        #endregion
        #region ExtractValue (SecureString)
        /// <summary>
        /// Extracts the actual string contents from a SecureString instance
        /// </summary>
        public static string ExtractValue(this SecureString value)
        {
            var bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
        #endregion
    }
}
